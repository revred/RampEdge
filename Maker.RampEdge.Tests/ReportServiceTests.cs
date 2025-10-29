using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class ReportServiceTests
    {
        ILogger<ReportService> logger = NullLogger<ReportService>.Instance;
        [Fact]
        public async Task AddProductReportAsync_WhenCalled_InvokesClientWithCorrectRequest()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

            AddProductReportRequest? capturedRequest = null;

            var file = new FileString { FileBytes = "abc123", FileName = "doc.txt" };

            var request = new AddProductReportRequest
            {
                ProductBarID = 111,
                Message = "This report describes test data",
                ReportType = "Bug",
                ReadMe = "Steps to reproduce...",
                ReadMeHtml = "<p>Steps to reproduce...</p>",
                Files = new List<FileString> { file }
            };

            mockClient.Setup(c => c.AddProductReportAsync(It.IsAny<AddProductReportRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                      .Callback<AddProductReportRequest, string?, CancellationToken>((r, k, ct) => capturedRequest = r)
                      .Returns(Task.CompletedTask);

            var sut = new ReportService(mockClient.Object, logger);

            // Act
            await sut.AddProductReportAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.ProductBarID.Should().Be(111);
            capturedRequest.ReportType.Should().Be("Bug");
            capturedRequest.Files.Should().ContainSingle(f => f.FileName == "doc.txt");
        }
        [Fact]
        public async Task GetReportByProductAsync_WhenCalled_ReturnsExpectedReports()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedResponse = new ReportListResponse
            {
                Reports = new List<Report>
        {
            new Report
            {
                BarID = 101,
                Message = "Performance issue found",
                ReportType = "Performance",
                ReadMe = "Memory spike under load",
                ReadMeHtml = "<p>Memory spike under load</p>",
                Assets = new List<DigitalAsset> { new DigitalAsset { Url = "https://example.com/screenshot.png", Is3DFile = false } }
            }
        }
            };

            var request = new ReportAndRatingRequest { BarID = 12345 };

            mockClient.Setup(c => c.GetReportByProductAsync(It.IsAny<ReportAndRatingRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

            var sut = new ReportService(mockClient.Object, logger);

            // Act
            var result = await sut.GetReportByProductAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Reports.Should().HaveCount(1);
            result.Reports.First().ReportType.Should().Be("Performance");
            result.Reports.First().Assets.Should().ContainSingle(a => a.Url == "https://example.com/screenshot.png");
        }
        [Fact]
        public async Task UpdateProductReportAsync_WhenCalled_InvokesClientWithCorrectRequest()
        {
            // Arrange
            var mockClient = new Mock<IMakerClient>(MockBehavior.Strict);

            UpdateProductReportRequest? capturedRequest = null;

            var request = new UpdateProductReportRequest
            {
                ReportBarID = 888,
                Message = "Updated report message",
                ReportType = "UI",
                ReadMe = "UI improvement suggestions",
                ReadMeHtml = "<p>UI improvement suggestions</p>",
                Files = new List<FileString>
        {
            new FileString { FileName = "screenshot.png", FileBytes = "xyz789" }
        }
            };

            mockClient.Setup(c => c.UpdateProductReportAsync(It.IsAny<UpdateProductReportRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                      .Callback<UpdateProductReportRequest, string?, CancellationToken>((r, k, ct) => capturedRequest = r)
                      .Returns(Task.CompletedTask);

            var sut = new ReportService(mockClient.Object, logger);

            // Act
            await sut.UpdateProductReportAsync(request);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.ReportBarID.Should().Be(888);
            capturedRequest.ReportType.Should().Be("UI");
            capturedRequest.Files.Should().ContainSingle(f => f.FileName == "screenshot.png");

            mockClient.Verify(c => c.UpdateProductReportAsync(It.IsAny<UpdateProductReportRequest>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
