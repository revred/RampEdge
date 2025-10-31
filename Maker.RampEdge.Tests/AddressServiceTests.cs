using FluentAssertions;
using Maker.RampEdge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maker.RampEdge.Tests
{
    public class AddressServiceTests
    {
        ILogger<AddressService> logger = NullLogger<AddressService>.Instance;
        [Fact]
        public async Task GetAddressAsync_WhenAddressesExist_ReturnsExpectedAddressDetails()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var expectedResponse = new AddressResponse
            {
                AddressDetails = new List<AddressDetails>
        {
            new()
            {
                BarID = 1001,
                Email = "john@example.com",
                Site = "Head Office",
                PhoneNumber = "1234567890",
                State = "California",
                Pincode = "90001",
                Country = "USA",
                Address = "123 Main Street"
            },
            new()
            {
                BarID = 1002,
                Email = "jane@example.com",
                Site = "Branch Office",
                PhoneNumber = "9876543210",
                State = "Texas",
                Pincode = "73301",
                Country = "USA",
                Address = "456 Elm Street"
            }
        }
            };

            mockRepo.Setup(r => r.GetAddressAsync(
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var sut = new AddressService(mockRepo.Object, logger);

            // Act
            var result = await sut.GetAddressAsync();

            // Assert
            result.Should().NotBeNull();
            result.AddressDetails.Should().HaveCount(2);

            var first = result.AddressDetails.First();
            first.Email.Should().Be("john@example.com");
            first.Site.Should().Be("Head Office");
            first.Country.Should().Be("USA");
            first.Address.Should().Contain("Main");

            var second = result.AddressDetails.Last();
            second.Email.Should().Be("jane@example.com");
            second.State.Should().Be("Texas");
        }
        [Fact]
        public async Task UpsertAddressAsync_WhenCalled_ReturnsSuccessResponse()
        {
            // Arrange
            var mockRepo = new Mock<IMakerClient>(MockBehavior.Strict);

            var addressDetails = new AddressDetails
            {
                BarID = 1001,
                Email = "john@example.com",
                Site = "Main Office",
                PhoneNumber = "1234567890",
                State = "California",
                Pincode = "90001",
                Country = "USA",
                Address = "123 Main Street"
            };

            var request = new AddressRequest
            {
                BarID = 1001,
                AddressDetails = addressDetails
            };

            var expectedResponse = new UpsertAddressResponse
            {
                IsSuccess = true
            };

            mockRepo.Setup(r => r.UpsertAddressAsync(
                    It.IsAny<AddressRequest>(),
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var sut = new AddressService(mockRepo.Object, logger);

            // Act
            var result = await sut.UpsertAddressAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

        }

    }
}
