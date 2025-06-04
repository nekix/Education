using Microsoft.AspNetCore.Mvc.Testing;

namespace FrameworksEducation.AspNetCore.Tests.Chapter_13;

public class WeatherForecast_Tests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;

    public WeatherForecast_Tests(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Controller_ReturnWeather()
    {
        HttpClient client = _fixture.CreateClient();

        var response = await client.GetAsync("web-api/WeatherForecast");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.True(content.Contains("temperatureC"));
    }
}