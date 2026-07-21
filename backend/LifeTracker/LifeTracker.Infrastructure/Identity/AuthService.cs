using System.Security.Claims;
using FluentValidation;
using LifeTracker.Application.DTOs.Auth;
using LifeTracker.Application.Exceptions;
using LifeTracker.Application.Interfaces.Services;
using LifeTracker.Domain.Common.Constants;
using LifeTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LifeTracker.Infrastructure.Identity;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<ApplicationRole> roleManager,
    ITokenService tokenService,
    AppDbContext dbContext,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator,
    IValidator<RefreshTokenRequest> refreshTokenValidator,
    IValidator<ResetPasswordRequest> resetPasswordValidator,
    IValidator<ChangePasswordRequest> changePasswordValidator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            Email = request.Email.Trim(),
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            CreatedOn = DateTimeOffset.UtcNow,
            IsActive = true,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, request.Password);
        EnsureSuccess(result, "Registration failed.");

        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new ApplicationRole(Roles.User)
            {
                Description = "Default application user"
            });
        }

        await userManager.AddToRoleAsync(user, Roles.User);

        return await BuildAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAppException("Invalid email or password.");

        if (!user.IsActive)
        {
            throw new UnauthorizedAppException("User account is inactive.");
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            throw new UnauthorizedAppException("User account is locked.");
        }

        if (!signInResult.Succeeded)
        {
            throw new UnauthorizedAppException("Invalid email or password.");
        }

        return await BuildAuthResponseAsync(user, ipAddress, cancellationToken);
    }

    public async Task LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new BadRequestException("Refresh token is required.");
        }

        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return;
        }

        storedToken.RevokedOn = DateTimeOffset.UtcNow;
        storedToken.RevokedByIp = ipAddress;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TokenResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        await refreshTokenValidator.ValidateAndThrowAsync(request, cancellationToken);

        var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedAppException("Invalid access token.");

        var userIdValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (!long.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAppException("Invalid access token.");
        }

        var storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(
                token => token.Token == request.RefreshToken && token.UserId == userId,
                cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            throw new UnauthorizedAppException("Invalid refresh token.");
        }

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new UnauthorizedAppException("User not found.");

        if (!user.IsActive)
        {
            throw new UnauthorizedAppException("User account is inactive.");
        }

        storedToken.RevokedOn = DateTimeOffset.UtcNow;
        storedToken.RevokedByIp = ipAddress;

        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetPermissionsAsync(user, roles);

        var tokens = await tokenService.GenerateTokensAsync(
            user.Id,
            user.Email!,
            roles,
            permissions,
            ipAddress,
            cancellationToken);

        storedToken.ReplacedByToken = tokens.RefreshToken;
        await dbContext.SaveChangesAsync(cancellationToken);

        return tokens;
    }

    public async Task<MessageResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new BadRequestException("Email is required.");
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return new MessageResponse
            {
                Message = "If the email exists, a password reset token has been generated."
            };
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        return new MessageResponse
        {
            Message = "If the email exists, a password reset token has been generated.",
            Token = token
        };
    }

    public async Task<MessageResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        await resetPasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.Email);

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        EnsureSuccess(result, "Password reset failed.");

        return new MessageResponse { Message = "Password has been reset successfully." };
    }

    public async Task<MessageResponse> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        await changePasswordValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        var result = await userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        EnsureSuccess(result, "Password change failed.");

        return new MessageResponse { Message = "Password changed successfully." };
    }

    public async Task<MessageResponse> ChangeEmailAsync(
        long userId,
        ChangeEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewEmail))
        {
            throw new BadRequestException("New email is required.");
        }

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException(nameof(ApplicationUser), userId);

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAppException("Invalid password.");
        }

        var existing = await userManager.FindByEmailAsync(request.NewEmail);
        if (existing is not null && existing.Id != user.Id)
        {
            throw new ConflictException("Email is already in use.");
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        var result = await userManager.ChangeEmailAsync(user, request.NewEmail, token);
        EnsureSuccess(result, "Email change failed.");

        user.UserName ??= request.NewEmail;
        await userManager.UpdateAsync(user);

        return new MessageResponse { Message = "Email changed successfully." };
    }

    public async Task<MessageResponse> VerifyEmailAsync(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Token))
        {
            throw new BadRequestException("User id and token are required.");
        }

        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        EnsureSuccess(result, "Email verification failed.");

        return new MessageResponse { Message = "Email verified successfully." };
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(
        ApplicationUser user,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetPermissionsAsync(user, roles);

        var tokens = await tokenService.GenerateTokensAsync(
            user.Id,
            user.Email!,
            roles,
            permissions,
            ipAddress,
            cancellationToken);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiresOn = tokens.AccessTokenExpiresOn,
            Roles = roles.ToArray(),
            Permissions = permissions.ToArray()
        };
    }

    private async Task<IReadOnlyCollection<string>> GetPermissionsAsync(
        ApplicationUser user,
        IList<string> roles)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var userClaims = await userManager.GetClaimsAsync(user);
        foreach (var claim in userClaims.Where(claim => claim.Type == Permissions.ClaimType))
        {
            permissions.Add(claim.Value);
        }

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var roleClaims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in roleClaims.Where(claim => claim.Type == Permissions.ClaimType))
            {
                permissions.Add(claim.Value);
            }
        }

        return permissions.ToArray();
    }

    private static void EnsureSuccess(IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(" ", result.Errors.Select(error => error.Description));
        throw new BadRequestException($"{message} {errors}".Trim());
    }
}
