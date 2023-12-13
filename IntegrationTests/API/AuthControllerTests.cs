
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using SolveChess.Logic.Interfaces;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SolveChess.DAL.Model;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using SolveChess.Logic.Models;
using SolveChess.Logic.Attributes;
using static Google.Apis.Requests.BatchRequest;

namespace SolveChess.API.IntegrationTests;

[TestClass]
public class AuthControllerTests
{

    private IJwtProvider _jwtProvider = null!;
    private SolveChessWebApplicationFactory _factory = null!;
    private AppDbContext _dbContext = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _factory = new SolveChessWebApplicationFactory();

        var scope = _factory.Services.CreateScope();
        _jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [TestMethod]
    public async Task GetUserIdTest_Returns200OkAndUserId()
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
    public async Task GetUserIdTest_Returns401Unauthorized()
    {
        //Arrange
        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync("/auth/userId");

        //Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GoogleLoginTest_Returns200OkAndJwtTokenCookieAndUserIsMadeInDatabase()
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

    [TestMethod]
    public async Task DoesUserExistTest_UserExistsReturns200OkAndUserId()
    {
        //Arrange
        var userId = "200";

        _dbContext.User.Add(new User() 
        { 
            Id = userId,
            Email = "test@example.com",
            AuthType = AuthType.GOOGLE
        });

        await _dbContext.SaveChangesAsync();

        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync($"/auth/user/{userId}");

        //Arrange
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(userId, await response.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task DoesUserExistTest_UserDoesntExistReturns404NotFound()
    {
        //Arrange
        var userId = "200";

        var client = _factory.CreateClient();

        //Act
        var response = await client.GetAsync($"/auth/user/{userId}");

        //Arrange
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

}