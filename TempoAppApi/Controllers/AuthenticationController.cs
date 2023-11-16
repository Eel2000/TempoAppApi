using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TempoAppApi.DTOs;
using TempoAppApi.Models;
using TempoAppApi.Services;

namespace TempoAppApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }


    [HttpPost("login")]
    [Produces(typeof(Response<AuthResponse>))]
    [ProducesResponseType(typeof(Response<AuthResponse>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<AuthResponse>),StatusCodes.Status401Unauthorized)]
    public async ValueTask<IActionResult> LoginAsync([FromBody, Required] AuthRequest request)
    {
        var auth = await _authenticationService.AuthenticateAsync(request);
        if (auth.Succeed)
            return Ok(auth);

        return Unauthorized(auth);
    }

    [HttpPost("register-user")]
    [Produces(typeof(Response<AuthResponse>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async ValueTask<IActionResult> RegisterAsync([FromBody, Required] AuthRegisterRequest request)
    {
        var registration = await _authenticationService.RegisterAsync(request);
        if (registration.Succeed)
            return Ok(registration);

        return BadRequest(registration);
    }
}