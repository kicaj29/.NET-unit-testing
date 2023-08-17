using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using VerifyCallsOrder.Controllers;
using static System.Net.Mime.MediaTypeNames;

namespace VerifyCallsOrder.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestOrder()
        {
            Mock<ILogger<WeatherForecastController>> loggerMock = new Mock<ILogger<WeatherForecastController>>();

            MockSequence seq = new MockSequence();

            // based on https://github.com/moq/moq/blob/main/src/Moq.Tests/Regressions/IssueReportsFixture.cs#L3796

            loggerMock.InSequence(seq).Setup(l => l.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((message, _) => message!.ToString().StartsWith("ab")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();

            loggerMock.InSequence(seq).Setup(l => l.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((message, _) => message!.ToString().StartsWith("12")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>())).Verifiable();


            WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton(loggerMock.Object);
                    }));
            HttpClient client = application.CreateClient();

            await client.GetAsync("WeatherForecast");

            loggerMock.Verify();
            loggerMock.VerifyNoOtherCalls();

            /*loggerMock.Verify(l => l.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((message, _) => message!.ToString().StartsWith("ab")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);*/

            Assert.Pass();
        }
    }
}