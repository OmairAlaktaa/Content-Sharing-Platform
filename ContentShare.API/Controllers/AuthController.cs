using ContentShare.API.SwaggerExamples.Auth;
using ContentShare.Application.DTOs.Auth;
using ContentShare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    private readonly IAuthService _auth = auth;

    [AllowAnonymous]
    [HttpPost("register")]
    [SwaggerRequestExample(typeof(RegisterRequest), typeof(RegisterRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthResponseExample))]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req, CancellationToken ct)
        => Ok(await _auth.RegisterAsync(req, ct));

    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthResponseExample))]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
        => Ok(await _auth.LoginAsync(req, ct));
}
