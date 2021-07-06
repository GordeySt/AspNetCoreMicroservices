using Catalog.API.Controllers;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests
{
    public class CatalogControllerTests
    {
        private readonly Mock<IProductRepository> _repositoryStub = new();
        private readonly Mock<ILogger<CatalogController>> _loggerStub = new();
        private readonly Random rand = new();

        [Fact]
        public async Task GetProductById_WithUnexistingProduct_ReturnsNotFound()
        {
            // Arrange
            _repositoryStub.Setup(repo => repo.GetProduct(It.IsAny<string>()))
                .ReturnsAsync((Product)null);


            var controller = new CatalogController(_repositoryStub.Object, 
                _loggerStub.Object);

            // Act
            var result = await controller.GetProductById(Guid.NewGuid().ToString());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetProductById_WithExistingProduct_ReturnsExpectedItem()
        {
            // Arrange
            var exptectedProduct = CreateRandomProduct();

            _repositoryStub.Setup(repo => repo.GetProduct(It.IsAny<string>()))
                .ReturnsAsync(exptectedProduct);

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var actionResult = await controller.GetProductById(Guid.NewGuid().ToString());

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var result = actionResult.Result as OkObjectResult;
            result.Value.Should().BeEquivalentTo(exptectedProduct);
        }

        [Fact]
        public async Task GetProducts_WithExistingItems_ReturnAllItems()
        {
            // Arrange
            var expectedItems = new[] 
            { 
                CreateRandomProduct(),
                CreateRandomProduct(),
                CreateRandomProduct()
            };

            _repositoryStub.Setup(repo => repo.GetProducts())
                .ReturnsAsync(expectedItems);

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var actionResult = await controller.GetProducts();

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var result = actionResult.Result as OkObjectResult;
            result.Value.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task CreateProduct_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            var productToCreate = CreateRandomProduct();

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var result = await controller.CreateProduct(productToCreate);

            // Assert
            var createdItem = (result.Result as CreatedAtRouteResult).Value as Product;
            productToCreate.Should().BeEquivalentTo(createdItem);
            createdItem.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateProduct_WithExistingItem_ReturnsTrue()
        {
            // Arrange
            var productToUpdate = CreateRandomProduct();
            _repositoryStub.Setup(repo => repo.UpdateProduct(productToUpdate))
                .ReturnsAsync(true);

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var actionResult = await controller.UpdateProduct(productToUpdate);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var result = actionResult as OkObjectResult;
            result.Value.Should().Be(true);
        }

        [Fact]
        public async Task UpdateProduct_WithUnexistingItem_ReturnsFalse()
        {
            // Arrange
            var productToUpdate = CreateRandomProduct();
            _repositoryStub.Setup(repo => repo.UpdateProduct(productToUpdate))
                .ReturnsAsync(false);

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var actionResult = await controller.UpdateProduct(productToUpdate);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteProduct_WithExistingItem_ReturnsTrue()
        {
            // Arrange
            _repositoryStub.Setup(repo => repo.DeleteProduct(It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new CatalogController(_repositoryStub.Object,
                _loggerStub.Object);

            // Act
            var actionResult = await controller.DeleteProduct(Guid.NewGuid().ToString());

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var result = actionResult as OkObjectResult;
            result.Value.Should().Be(true);
        }

        private Product CreateRandomProduct()
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Category = Guid.NewGuid().ToString(),
                Summary = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ImageFile = Guid.NewGuid().ToString(),
                Price = rand.Next(1000)
            };
        }
    }
}
