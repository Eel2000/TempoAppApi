using Microsoft.AspNetCore.Identity;
using TempoAppApi.DTOs;

namespace TempoAppApi.Extensions;

public static class IdentityUserExtension
{
    public static AuthResponse GetUser(this IdentityUser user, string? token = null)
    {
        return new AuthResponse(user.UserName, user.Email, user.Id, token);
    }
}