using Basket.API.Controllers;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Basket.UnitTests
{
    public class BasketControllerTests
    {
        private readonly Mock<IBasketRepository> _repositoryStub = new();
        private readonly Mock<DiscountGrpcService> _discountGrpcStub = new();

        [Fact]
        public async Task GetBasket_WithExistingBasket_ReturnsExpectedBasket()
        {
            // Arrange
            var expectedBasket = CreateRandomBasket();

            _repositoryStub.Setup(repo => repo.GetBasket(It.IsAny<string>()))
                .ReturnsAsync(expectedBasket);

            var controller = new BasketController(_repositoryStub.Object,
                _discountGrpcStub.Object);

            // Act
            var actionResult = await controller.GetBasket(Guid.NewGuid().ToString());

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var result = actionResult.Result as OkObjectResult;
            result.Value.Should().BeEquivalentTo(expectedBasket);
        }

        [Fact]
        public async Task UpdateBasket_WithItemToUpdate_ReturnsOK()
        {
            // Arrange
            var basketToUpdate = CreateRandomBasket();

            var controller = new BasketController(_repositoryStub.Object,
                _discountGrpcStub.Object);

            // Act
            var result = await controller.UpdateBasket(basketToUpdate);

            // Assert
            var updatedItem = (result.Result as CreatedAtRouteResult).Value as ShoppingCart;
            basketToUpdate.Should().BeEquivalentTo(updatedItem);
            updatedItem.UserName.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteBasket_WithExistingItem_ReturnsOK()
        {
            // Arrange
            var controller = new BasketController(_repositoryStub.Object,
                _discountGrpcStub.Object);

            // Act
            var result = await controller.DeleteBasket(Guid.NewGuid().ToString());

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        private ShoppingCart CreateRandomBasket()
        {
            return new()
            {
                UserName = Guid.NewGuid().ToString(),
                Items = new()
            };
        }
    }
}
