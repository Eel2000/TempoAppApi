using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TempoAppApi.Models;

namespace TempoAppApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    [HttpGet("/weatherforecast")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces(typeof(WeatherForecast))]
    public IActionResult GetWeatherForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
            "Lubumbashi"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();

        return Ok(forecast);
    }
}