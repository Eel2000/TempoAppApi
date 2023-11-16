using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TempoAppApi.DTOs;
using TempoAppApi.Extensions;
using TempoAppApi.Models;

namespace TempoAppApi.Services;

public class AuthenticationService : IAuthenticationService
{
    private UserManager<IdentityUser> _userManager;
    private SignInManager<IdentityUser> _signInManager;
    private IConfiguration _configuration;
    private ILogger<AuthenticationService> _logger;

    public AuthenticationService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }


    public async ValueTask<Response<AuthResponse>> AuthenticateAsync(AuthRequest request)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                user = await _userManager.FindByEmailAsync(request.Username);
            if (user is null)
                return new Response<AuthResponse>("Account not found!", new string[] { });

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User logged in");
                var token = GenerateToken(user);
                return new Response<AuthResponse>($"AUTH_004: User authenticated successfully!", user.GetUser(token));
            }
            else if (signInResult.IsLockedOut)
            {
                _logger.LogWarning("User failed to log in: Account blocked");
                return new Response<AuthResponse>("You're Account has been blocked", user.GetUser());
            }
            else if (signInResult.IsNotAllowed)
            {
                _logger.LogWarning("Account not allowed");
                return new Response<AuthResponse>("Account not allowed", user.GetUser());
            }
            else if (signInResult.RequiresTwoFactor)
            {
                _logger.LogWarning("You're account login process requires two-fa authentication");
                return new Response<AuthResponse>("You're account login process requires two-fa authentication",
                    user.GetUser());
            }

            return new Response<AuthResponse>(
                "Unknown Error occured. Please check your account credentials and retry or contact the admin for further assist",
                user.GetUser());
        }
        catch (Exception e)
        {
            _logger.LogError(new(500), e, "An error occured while processing");
            return new Response<AuthResponse>(e.Message, new[] { e.Message, e?.InnerException?.Message });
        }
    }

    public async ValueTask<Response<AuthResponse>> RegisterAsync(AuthRegisterRequest request)
    {
        try
        {
            if (_userManager.Users.Any(x => x.UserName == request.Username))
            {
                _logger.LogError("This user name is taken");
                return new Response<AuthResponse>("AUTH_002: Username already taken. please chose another one");
            }

            IdentityUser user = new()
            {
                UserName = request.Username,
                Email = request.Email,
            };

            var registrationResult = await _userManager.CreateAsync(user, request.Password);
            if (registrationResult.Succeeded)
            {
                _logger.LogInformation("AUTH_000: Account created");
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("AUTH_003: User logged in");
                    var token = GenerateToken(user);
                    return new Response<AuthResponse>($"AUTH_TOKEN: {token}", user.GetUser(token));
                }
            }

            _logger.LogError("Failed to created user");

            var errors = registrationResult.Errors.Select(x => x.Description).ToArray();
            return new Response<AuthResponse>("AUTH_001: Failed to register user", errors);
        }
        catch (Exception e)
        {
            _logger.LogError(new(500), e, "An error occured while processing");
            return new Response<AuthResponse>(e.Message, new[] { e.Message, e?.InnerException?.Message });
        }
    }


    private string GenerateToken(IdentityUser user)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var tokenExpiryTimeInMinutes = Convert.ToInt64(_configuration["JWT:expiry"]);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            // new (ClaimTypes.Role,"Admin")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JWT:Issuer"],
            Audience = _configuration["JWT:Audience"],
            Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTimeInMinutes),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}