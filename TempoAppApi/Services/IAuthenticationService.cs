using Microsoft.AspNetCore.Identity;
using TempoAppApi.DTOs;
using TempoAppApi.Models;

namespace TempoAppApi.Services;

public interface IAuthenticationService
{
    ValueTask<Response<AuthResponse>> AuthenticateAsync(AuthRequest request);
    ValueTask<Response<AuthResponse>> RegisterAsync(AuthRegisterRequest request);
}