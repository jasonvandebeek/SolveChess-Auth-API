
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using SolveChess.Logic.Interfaces;

namespace SolveChess.API.IntegrationTests;

[TestClass]
public class AuthControllerTests
{

    private readonly SolveChessWebApplicationFactory _factory;

    public AuthControllerTests()
    {
        _factory = new SolveChessWebApplicationFactory();
    }

    [TestMethod]
    public async Task GetUserId_Returns_UserId_When_UserIsAuthenticated()
    {
        // Arrange
        var userId = "testUserId";

        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        var jwtToken = jwtProvider.GenerateToken(userId);
        client.DefaultRequestHeaders.Add("Cookie", $"AccessToken={jwtToken}");

        // Act
        var response = await client.GetAsync("/auth/userId");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.AreEqual(userId, responseBody);
    }

}