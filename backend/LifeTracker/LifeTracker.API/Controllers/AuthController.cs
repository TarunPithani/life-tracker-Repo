using LifeTracker.Application.DTOs.Auth;
using LifeTracker.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeTracker.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(
            request,
            GetIpAddress(),
            cancellationToken);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(
            request,
            GetIpAddress(),
            cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(
            request.RefreshToken,
            GetIpAddress(),
            cancellationToken);

        return Ok(new MessageResponse { Message = "Logged out successfully." });
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RefreshTokenAsync(
            request,
            GetIpAddress(),
            cancellationToken);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.ResetPasswordAsync(request, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await authService.ChangePasswordAsync(
            currentUser.UserId.Value,
            request,
            cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpPost("change-email")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> ChangeEmail(
        [FromBody] ChangeEmailRequest request,
        [FromServices] ICurrentUserService currentUser,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Unauthorized();
        }

        var response = await authService.ChangeEmailAsync(
            currentUser.UserId.Value,
            request,
            cancellationToken);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MessageResponse>> VerifyEmail(
        [FromBody] VerifyEmailRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.VerifyEmailAsync(request, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me([FromServices] ICurrentUserService currentUser)
    {
        return Ok(new
        {
            currentUser.UserId,
            currentUser.Email,
            currentUser.IsAuthenticated,
            currentUser.Roles,
            currentUser.Permissions
        });
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.FirstOrDefault();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
