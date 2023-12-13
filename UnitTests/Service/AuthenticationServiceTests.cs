using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolveChess.Logic.DAL;
using SolveChess.Logic.Models;
using SolveChess.Logic.Interfaces;
using System.Net;
using Moq;
using Moq.Protected;
using SolveChess.Logic.Exceptions;
using SolveChess.Logic.Attributes;
using System.Net.Http;

namespace SolveChess.Logic.Service.Tests;

[TestClass()]
public class AuthenticationServiceTests
{

    [TestMethod]
    public async Task AuthenticateGoogle_ValidAccessToken_ReturnsToken()
    {
        //Arrange
        var authenticationDalMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var accessToken = "validAccessToken";
        var expectedToken = "generatedToken";

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"email\": \"test@example.com\"}")
            });

        authenticationDalMock.Setup(dal => dal.GetUserWithEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)new User { Id = "1", Email = "test@example.com", AuthType = AuthType.GOOGLE });

        jwtProviderMock
            .Setup(provider => provider.GenerateToken(It.IsAny<string>()))
            .Callback<string>(userId =>
            {
                Assert.AreEqual("1", userId);
            })
            .Returns(expectedToken);

        var service = new AuthenticationService(authenticationDalMock.Object, jwtProviderMock.Object, httpClient);

        //Act
        var result = await service.AuthenticateGoogle(accessToken);

        //Assert
        Assert.AreEqual(expectedToken, result);
    }

    [TestMethod]
    public void AuthenticateGoogle_InvalidAccessToken_ThrowsException()
    {
        //Arrange
        var authenticationDalMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var accessToken = "invalidAccessToken";

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        var service = new AuthenticationService(authenticationDalMock.Object, jwtProviderMock.Object, httpClient);

        //Assert
        Assert.ThrowsExceptionAsync<AuthenticationException>(async () =>
        {
            //Act
            var result = await service.AuthenticateGoogle(accessToken);
        });
    }

    [TestMethod]
    public async Task AuthenticateGoogleTest_NoEmailInGoogleResponse_ReturnsNull()
    {
        //Arrange
        var authenticationDalMock = new Mock<IAuthenticationDal>();
        var jwtProviderMock = new Mock<IJwtProvider>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        
        var accessToken = "invalidAccessToken";

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });

        var service = new AuthenticationService(authenticationDalMock.Object, jwtProviderMock.Object, httpClient);

        //Act
        var result = await service.AuthenticateGoogle(accessToken);

        //Assert
        Assert.AreEqual(null, result);
    }

    [TestMethod]
    public async Task DoesUserExistTest()
    {
        //Arrange
        var authenticationDalMock = new Mock<IAuthenticationDal>();
        authenticationDalMock.Setup(dal => dal.GetUserWithId(It.IsAny<string>()))
            .ReturnsAsync((User?)new User { Id = "1", Email = "test@example.com", AuthType = AuthType.GOOGLE });

        var jwtProviderMock = new Mock<IJwtProvider>();

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var service = new AuthenticationService(authenticationDalMock.Object, jwtProviderMock.Object, httpClient);

        //Act
        var result = await service.DoesUserExist("1");

        //Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task DoesUserExistTest_UserDoesntExist()
    {
        //Arrange
        var authenticationDalMock = new Mock<IAuthenticationDal>();
        authenticationDalMock.Setup(dal => dal.GetUserWithId(It.IsAny<string>()))
            .ReturnsAsync(null as User);

        var jwtProviderMock = new Mock<IJwtProvider>();

        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);

        var service = new AuthenticationService(authenticationDalMock.Object, jwtProviderMock.Object, httpClient);

        //Act
        var result = await service.DoesUserExist("1");

        //Assert
        Assert.IsFalse(result);
    }

}