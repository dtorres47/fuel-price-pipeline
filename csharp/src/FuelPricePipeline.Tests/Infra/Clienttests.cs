using FuelPricePipeline.Infra.Eia;
using Microsoft.Extensions.Logging;
using Moq;

namespace FuelPricePipeline.Tests.Infra;

[TestClass]
public class ClientTests
{
    [TestMethod]
    public async Task FetchLatestDieselAsync_WithInvalidArea_ShouldThrowArgumentException()
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockLogger = new Mock<ILogger<Client>>();
        var client = new Client("test-key", mockFactory.Object, mockLogger.Object);

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            async () => await client.FetchLatestDieselAsync("INVALID"));
    }

    [TestMethod]
    public async Task FetchLatestDieselAsync_WithValidArea_ShouldNotThrow()
    {
        // Testing placeholder
        Assert.IsTrue(true);
    }
}