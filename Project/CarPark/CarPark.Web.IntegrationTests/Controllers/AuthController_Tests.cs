using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using CarPark.Controllers;

namespace CarPark.Web.IntegrationTests.Controllers;

public class AuthController_Tests
{
    [Fact]
    public async Task Login_SendRequestWithoutCsrf_ReturnBadRequest()
    {
        // Arrange
        WebApplicationFactory<Program> webHostFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        HttpClient client = webHostFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        Dictionary<string, string> postData = new Dictionary<string, string>
        {
            { "Username", "manager1" },
            { "Password", "123456" }
        };

        // Act
        HttpResponseMessage response = await client.PostAsync("auth/login",
            new FormUrlEncodedContent(postData));

        string content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(string.IsNullOrEmpty(content));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_SendRequestWithCsrf_ReturnFound()
    {
        // Arrange
        WebApplicationFactory<Program> webHostFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });

        HttpClient client = webHostFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Get CSRF token
        HttpResponseMessage csrfResponse = await client.GetAsync("auth/login");
        List<string> cookies = csrfResponse.Headers.GetValues("Set-Cookie").ToList();

        string htmlContent = await csrfResponse.Content.ReadAsStringAsync();
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(htmlContent));
        IHtmlInputElement? tokenInput = document.QuerySelector<IHtmlInputElement>("input[name='__RequestVerificationToken']");
        string token = tokenInput?.Value ?? throw new InvalidOperationException("CSRF token not found");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Cookie", cookies);

        Dictionary<string, string> postData = new Dictionary<string, string>
        {
            { "Username", "manager1" },
            { "Password", "123456" },
            { "__RequestVerificationToken", token}
        };

        // Act
        HttpResponseMessage response = await client.PostAsync("auth/login",
            new FormUrlEncodedContent(postData));

        // Assert
        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
    }

    [Fact]
    public void Login_CheckAntiCsrfAttr_HasAttribute()
    {
        // Arrange
        Type controllerType = typeof(AuthController);
        
        // Act
        MethodInfo? loginMethod = controllerType.GetMethods()
            .FirstOrDefault(m => m.Name == "Login" && 
                                 m.GetCustomAttribute<HttpPostAttribute>() != null);

        ValidateAntiForgeryTokenAttribute? csrfAttribute = loginMethod?.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();
        
        // Assert
        Assert.NotNull(loginMethod);
        Assert.NotNull(csrfAttribute);
    }
}