
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using SolveChess.Logic.Interfaces;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SolveChess.DAL.Model;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Text;

namespace SolveChess.API.IntegrationTests;

[TestClass]
public class AuthControllerTests
{

    private readonly IJwtProvider _jwtProvider;
    private readonly SolveChessWebApplicationFactory _factory;
    private readonly AppDbContext _dbContext;

    public AuthControllerTests()
    {
        _factory = new SolveChessWebApplicationFactory();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .ConfigureWarnings(warnings =>
            {
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning);
            });

        _dbContext = new AppDbContext(optionsBuilder.Options);

        using var scope = _factory.Services.CreateScope();
        _jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
    }

    [TestMethod]
    public async Task GetUserId_Returns200OkAndUserId()
    {
        //Arrange
        var userId = "testUserId";
        var jwtToken = _jwtProvider.GenerateToken(userId);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Cookie", $"AccessToken={jwtToken}");

        //Act
        var response = await client.GetAsync("/auth/userId");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(userId, responseBody);
    }

    [TestMethod]
    public async Task GoogleLogin_Returns200OkAndJwtTokenCookieAndUserIsMadeInDatabase()
    {
        //Arrange
        var fakeGoogleAccessToken = new
        {
            accessToken = "FakeGoogleAccessToken"
        };

        string jsonBody = JsonConvert.SerializeObject(fakeGoogleAccessToken);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var client = _factory.CreateClient();

        //Act
        var response = await client.PostAsync("/auth/google-login", content);
        response.EnsureSuccessStatusCode();

        var user = await _dbContext.User.Where(u => u.Email == "test@example.com").FirstOrDefaultAsync();

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(user);

        var cookies = response.Headers.GetValues("Set-Cookie");
        Assert.IsTrue(cookies.Any(cookie => cookie.Contains("AccessToken")));
    }

}