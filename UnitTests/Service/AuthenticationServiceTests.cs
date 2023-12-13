using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolveChess.Logic.DAL;
using SolveChess.Logic.Models;
using SolveChess.Logic.Interfaces;
using System.Net;
using Moq;
using Moq.Protected;
using SolveChess.Logic.Exceptions;

namespace SolveChess.Logic.Service.Tests;

[TestClass()]
public class AuthenticationServiceTests
{

    [TestMethod]
    public async Task AuthenticateGoogle_ValidAccessToken_ReturnsToken()
    {
        // Arrange
        var authenticationDALMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        var service = new AuthenticationService(authenticationDALMock.Object, jwtProviderMock.Object, httpClient);

        var accessToken = "validAccessToken";
        var expectedToken = "generatedToken";

        // Simulate a successful response from the Google API
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"email\": \"test@example.com\"}")
            });

        // Simulate a user being retrieved from the data access layer
        authenticationDALMock.Setup(dal => dal.GetUserWithEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)new User { Id = "1", Email = "test@example.com" });

        // Simulate token generation and capture the arguments
        jwtProviderMock
            .Setup(provider => provider.GenerateToken(It.IsAny<string>()))
            .Callback<string>(userId =>
            {
                // Assert that the GenerateToken method was called with the correct user ID
                Assert.AreEqual("1", userId);
            })
            .Returns(expectedToken);

        // Act
        var result = await service.AuthenticateGoogle(accessToken);

        // Assert
        Assert.AreEqual(expectedToken, result);
    }

    [TestMethod]
    public void AuthenticateGoogle_InvalidAccessToken_ThrowsException()
    {
        // Arrange
        var authenticationDALMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        var service = new AuthenticationService(authenticationDALMock.Object, jwtProviderMock.Object, httpClient);

        var accessToken = "invalidAccessToken";

        // Simulate an unsuccessful response from the Google API
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        // Assert
        Assert.ThrowsExceptionAsync<AuthenticationException>(async () =>
        {
            // Act
            var result = await service.AuthenticateGoogle(accessToken);
        });
    }

    [TestMethod]
    public async Task AuthenticateGoogle_NoEmailInGoogleResponse_ReturnsNull()
    {
        // Arrange
        var authenticationDALMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        var service = new AuthenticationService(authenticationDALMock.Object, jwtProviderMock.Object, httpClient);

        var accessToken = "invalidAccessToken";

        // Simulate an unsuccessful response from the Google API
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        // Act
        var result = await service.AuthenticateGoogle(accessToken);

        // Assert
        Assert.AreEqual(null, result);
    }

}