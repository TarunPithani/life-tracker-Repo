using LifeTracker.Application.DTOs.Auth;

namespace LifeTracker.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<TokenResponse> RefreshTokenAsync(
        RefreshTokenRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> ChangeEmailAsync(
        long userId,
        ChangeEmailRequest request,
        CancellationToken cancellationToken = default);

    Task<MessageResponse> VerifyEmailAsync(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default);
}
