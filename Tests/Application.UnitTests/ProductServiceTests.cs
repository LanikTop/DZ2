using Application.DTOs;
using Application.Services;
using Domain.Models;
using Infrastructure.DAO;
using Moq;
using NUnit.Framework;

namespace Application.UnitTests;

[TestFixture]
public sealed class ProductServiceTests
{
    private Mock<IProductDAO> _productDAO = null!;
    private ProductService _productService = null!;

    [SetUp]
    public void SetUp()
    {
        _productDAO = new Mock<IProductDAO>();
        _productService = new ProductService(_productDAO.Object);
    }

    [Test]
    public async Task CreateProductAsync_SavesProductAndReturnsGeneratedId()
    {
        var request = new CreateProductRequestDTO
        {
            Name = "Чайник",
            Price = 1500m,
            Weight = 1.5m,
            Type = ProductType.Electronics,
            WarehouseNumber = 2
        };

        var result = await _productService.CreateProductAsync(request);

        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        _productDAO.Verify(dao => dao.AddProductAsync(It.Is<Product>(product =>
            product.Id == result.Id &&
            product.Name == request.Name &&
            product.Price == request.Price &&
            product.Weight == request.Weight &&
            product.Type == request.Type &&
            product.WarehouseNumber == request.WarehouseNumber)), Times.Once);
    }

    [Test]
    public async Task GetProductByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _productDAO.Setup(dao => dao.GetProductByIdAsync(id)).ReturnsAsync((Product?)null);

        var result = await _productService.GetProductByIdAsync(id);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetProductsAsync_PassesFiltersAndPaginationToDao()
    {
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
        var request = new GetProductsRequestDTO
        {
            CreatedAt = createdAt,
            Type = ProductType.Food,
            WarehouseNumber = 4,
            Page = 2,
            PageSize = 10
        };
        var product = CreateProduct(ProductType.Food);
        _productDAO.Setup(dao => dao.GetProductsAsync(createdAt, ProductType.Food, 4, 2, 10))
            .ReturnsAsync(new[] { product });

        var result = await _productService.GetProductsAsync(request);

        Assert.That(result.Products, Has.Length.EqualTo(1));
        Assert.That(result.Products[0].Id, Is.EqualTo(product.Id));
        Assert.That(result.TotalCount, Is.EqualTo(1));
        _productDAO.Verify(dao => dao.GetProductsAsync(createdAt, ProductType.Food, 4, 2, 10), Times.Once);
    }

    [Test]
    public async Task UpdateProductAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var request = new UpdateProductRequestDTO { Id = Guid.NewGuid(), NewPrice = 100m };
        _productDAO.Setup(dao => dao.UpdateProductAsync(
                request.Id, request.NewName, request.NewPrice, request.NewWeight,
                request.NewType, request.NewWarehouseNumber))
            .ReturnsAsync((Product?)null);

        var result = await _productService.UpdateProductAsync(request);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateProductAsync_WhenProductExists_ReturnsUpdatedProduct()
    {
        var request = new UpdateProductRequestDTO
        {
            Id = Guid.NewGuid(),
            NewName = "Обновлённый товар",
            NewPrice = 200m
        };
        var updatedProduct = new Product
        {
            Id = request.Id,
            Name = request.NewName,
            Price = request.NewPrice.Value,
            Weight = 1m,
            Type = ProductType.General,
            CreatedAt = DateTimeOffset.UtcNow,
            WarehouseNumber = 1
        };
        _productDAO.Setup(dao => dao.UpdateProductAsync(
                request.Id, request.NewName, request.NewPrice, request.NewWeight,
                request.NewType, request.NewWarehouseNumber))
            .ReturnsAsync(updatedProduct);

        var result = await _productService.UpdateProductAsync(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(request.Id));
        Assert.That(result.Name, Is.EqualTo(request.NewName));
        Assert.That(result.Price, Is.EqualTo(request.NewPrice.Value));
    }

    private static Product CreateProduct(ProductType type) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Товар",
        Price = 100m,
        Weight = 1m,
        Type = type,
        CreatedAt = DateTimeOffset.UtcNow,
        WarehouseNumber = 1
    };
}
