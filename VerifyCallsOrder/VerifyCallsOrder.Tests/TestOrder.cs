using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using VerifyCallsOrder.Controllers;

namespace VerifyCallsOrder.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestOrderSimpleLogging()
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
                        services.AddSingleton<ILogger<WeatherForecastController>>(loggerMock.Object);
                    }));
            HttpClient client = application.CreateClient();

            await client.GetAsync("WeatherForecast/SimpleLogging");

            loggerMock.Verify();
            loggerMock.VerifyNoOtherCalls();

            Assert.Pass();
        }


        /// <summary>
        /// This test fails with message:
        ///     This mock failed verification due to the following unverified invocations:
        ///     ILogger.Log<FormattedLogValues>(LogLevel.Information, 0, abc: YXZ, null, Func<FormattedLogValues, Exception, string>)
        ///     ILogger.Log<FormattedLogValues>(LogLevel.Information, 0, 123: 999, null, Func<FormattedLogValues, Exception, string>)
        /// We can see that it uses FormattedLogValues which is placed in Microsoft.Extensions.Logging.Internal so probably we do not have access to it.
        /// https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.internal.formattedlogvalues?view=dotnet-plat-ext-2.2&viewFallbackFrom=net-6.0
        /// We can see that here we cannot use the same approach like in test TestOrderSimpleLogging.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestOrderLoggingWithParamsFailing()
        {
            Mock<ILogger<WeatherForecastController>> loggerMock = new Mock<ILogger<WeatherForecastController>>();

            MockSequence seq = new MockSequence();

            // based on https://github.com/moq/moq/blob/main/src/Moq.Tests/Regressions/IssueReportsFixture.cs#L3796
            loggerMock.InSequence(seq).Setup(l => l.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((message, _) => message!.ToString().StartsWith("abc: XY")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>())).Verifiable();

            loggerMock.InSequence(seq).Setup(l => l.Log(LogLevel.Information, 0,
                It.Is<It.IsAnyType>((message, _) => message!.ToString().StartsWith("123: 99")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>())).Verifiable();


            WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton<ILogger<WeatherForecastController>>(loggerMock.Object);
                    }));
            HttpClient client = application.CreateClient();

            await client.GetAsync("WeatherForecast/LoggingWithParams");

            loggerMock.Verify();
            loggerMock.VerifyNoOtherCalls();

            Assert.Pass();
        }

        [Test]
        public async Task TestOrderLoggingWithParamsPassing()
        {
            InMemoryFakeLogger<WeatherForecastController> loggerMock = new InMemoryFakeLogger<WeatherForecastController>();

            WebApplicationFactory<Program> application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddSingleton<ILogger<WeatherForecastController>>(loggerMock);
                    }));
            HttpClient client = application.CreateClient();

            await client.GetAsync("WeatherForecast/LoggingWithParams");

            Assert.AreEqual(2, loggerMock.Messages.Count);
            Assert.AreEqual("abc: YXZ", loggerMock.Messages[0]);
            Assert.AreEqual("123: 999", loggerMock.Messages[1]);

            Assert.Pass();
        }
    }
}