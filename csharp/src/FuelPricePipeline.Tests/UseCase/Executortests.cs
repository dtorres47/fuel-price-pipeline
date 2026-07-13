using FuelPricePipeline.Domain;
using FuelPricePipeline.Infra.Eia;
using FuelPricePipeline.Infra.Postgres;
using FuelPricePipeline.UseCase.GetLatest;
using Moq;

namespace FuelPricePipeline.Tests.UseCase;

[TestClass]
public class ExecutorTests
{
    private Mock<Client> _mockClient = null!;
    private Mock<Repo> _mockRepo = null!;
    private Mock<Microsoft.Extensions.Logging.ILogger<Executor>> _mockLogger = null!;
    private Executor _executor = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockClient = new Mock<Client>();
        _mockRepo = new Mock<Repo>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<Executor>>();
        _executor = new Executor(_mockClient.Object, _mockRepo.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenClientReturnsData_ShouldReturnSuccess()
    {
        // Arrange
        var expectedFuelRate = new DieselFuelPrice
        {
            ProductCode = "EPD2D",
            ProductName = "No 2 Diesel",
            AreaCode = "NUS",
            AreaName = "U.S.",
            Period = new DateTime(2025, 8, 1),
            Value = 3.744m,
            Unit = "$/GAL"
        };

        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedFuelRate);

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("EPD2D", result.Data.ProductCode);
        Assert.AreEqual(3.744m, result.Data.Value);

        _mockRepo.Verify(r => r.UpsertAsync(It.IsAny<DieselFuelPrice>()), Times.Once);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenClientReturnsNull_ShouldReturnFailure()
    {
        // Arrange
        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ReturnsAsync((DieselFuelPrice?)null);

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Failed to get data"));

        _mockRepo.Verify(r => r.UpsertAsync(It.IsAny<DieselFuelPrice>()), Times.Never);
    }

    [TestMethod]
    public async Task ExecuteAsync_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        _mockClient.Setup(c => c.FetchLatestDieselAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _executor.ExecuteAsync("test.csv", "NUS");

        // Assert
        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Unexpected error"));
    }
}