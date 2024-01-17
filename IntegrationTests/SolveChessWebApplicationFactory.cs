using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Moq.Protected;
using SolveChess.DAL.Model;
using System.Net;

namespace SolveChess.API.IntegrationTests;

internal class SolveChessWebApplicationFactory : WebApplicationFactory<Program>
{

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            var httpClientDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(HttpClient));

            if (httpClientDescriptor != null)
            {
                services.Remove(httpClientDescriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
                options.ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                });
            });

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Default);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
               {
                   Content = new StringContent("{\"email\": \"test@example.com\"}")
               })
               .Verifiable();

            services.AddScoped(_ => httpClient);

            Environment.SetEnvironmentVariable("SolveChess_CorsUrls", "https://localhost:3000");
            Environment.SetEnvironmentVariable("SolveChess_JwtSecret", "TestSecretKeyForJwtTokensInIntegrationTests");
        });
    }

}

