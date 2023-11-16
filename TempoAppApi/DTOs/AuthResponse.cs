namespace TempoAppApi.DTOs;

public record AuthResponse(string Username, string Email, string UserId, string? Token);