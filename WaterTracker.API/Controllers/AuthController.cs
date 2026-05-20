using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Runtime.InteropServices;
using WaterTracker.Contracts.Authentication;
using WaterTracker.Core.Interfaces;
using WaterTracker.Infrastructure.Identity;

namespace WaterTracker.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        var (token, expiresAtUtc) = _tokenService.CreateToken(user.Id, user.Email!, user.DisplayName);

        return Ok(new AuthResponse(
            UserId: user.Id,
            Email: user.Email!,
            DisplayName: user.DisplayName,
            Token: token,
            ExpiresAtUtc: expiresAtUtc));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        // intentionally identical response for missing user and wrong password to prevent users identifying registered users (OWASP)
        if (user is null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var (token, expiresAtUtc) = _tokenService.CreateToken(user.Id, user.Email!, user.DisplayName);

        return Ok(new AuthResponse(
            UserId: user.Id,
            Email: user.Email!,
            DisplayName: user.DisplayName,
            Token: token,
            ExpiresAtUtc: expiresAtUtc));
    }
}
