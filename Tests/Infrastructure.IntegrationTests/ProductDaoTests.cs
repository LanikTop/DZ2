using Dapper;
using Domain.Models;
using FluentMigrator.Runner;
using Infrastructure.DAO;
using Infrastructure.Helpers;
using Infrastructure.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Infrastructure.IntegrationTests;

[TestFixture]
[NonParallelizable]
public sealed class ProductDaoTests
{
    private ProductDAO _productDAO = null!;
    private IPostgresConnectionFactory _connectionFactory = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        _connectionFactory = new PostgresConnectionFactory(configuration);
        _productDAO = new ProductDAO(_connectionFactory);

        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException(
                "Connection string 'Postgres' не найден в appsettings.Test.json.");

        Migrations.MigrationRunner.RunMigrations(connectionString);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var connection = _connectionFactory.GetConnection();
        await connection.ExecuteAsync("DELETE FROM product_model");
    }

    [Test]
    public async Task AddProductAsync_ThenGetProductByIdAsync_ReturnsProduct()
    {
        var product = CreateProduct();

        await _productDAO.AddProductAsync(product);
        var result = await _productDAO.GetProductByIdAsync(product.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(product.Id));
        Assert.That(result!.Name, Is.EqualTo(product.Name));
        Assert.That(result.Price, Is.EqualTo(product.Price));
        Assert.That(result.Type, Is.EqualTo(product.Type));
        Assert.That(result.WarehouseNumber, Is.EqualTo(product.WarehouseNumber));
    }

    [Test]
    public async Task GetProductsAsync_AppliesFiltersAndPagination()
    {
        await _productDAO.AddProductAsync(CreateProduct("Food", ProductType.Food, 1));
        await _productDAO.AddProductAsync(CreateProduct("Electronics", ProductType.Electronics, 2));
        await _productDAO.AddProductAsync(CreateProduct("Other", ProductType.General, 2));

        var result = await _productDAO.GetProductsAsync(null, null, 2, 1, 1);
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].WarehouseNumber, Is.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo("Other"));
        Assert.That(result[0].Type, Is.EqualTo(ProductType.General));

        result = await _productDAO.GetProductsAsync(null, ProductType.Food, null, 1, 1);
        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].WarehouseNumber, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("Food"));
        Assert.That(result[0].Type, Is.EqualTo(ProductType.Food));
    }

    [Test]
    public async Task UpdateProductAsync_WhenProductExists_ReturnsUpdatedProduct()
    {
        var product = CreateProduct();
        await _productDAO.AddProductAsync(product);

        var result = await _productDAO.UpdateProductAsync(product.Id, "Новое имя", 250m, null, ProductType.Food, null);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Новое имя"));
        Assert.That(result.Price, Is.EqualTo(250m));
        Assert.That(result.Weight, Is.EqualTo(product.Weight));
        Assert.That(result.Type, Is.EqualTo(ProductType.Food));
        Assert.That(result.WarehouseNumber, Is.EqualTo(product.WarehouseNumber));

        result = await _productDAO.UpdateProductAsync(product.Id, null, null, 70m, null, 2);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Новое имя"));
        Assert.That(result.Price, Is.EqualTo(250m));
        Assert.That(result.Weight, Is.EqualTo(70m));
        Assert.That(result.Type, Is.EqualTo(ProductType.Food));
        Assert.That(result.WarehouseNumber, Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateProductAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var result = await _productDAO.UpdateProductAsync(
            Guid.NewGuid(), "Новое имя", null, null, null, null);

        Assert.That(result, Is.Null);
    }

    private static Product CreateProduct(
        string name = "Товар",
        ProductType type = ProductType.General,
        int warehouseNumber = 1) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Price = 100m,
        Weight = 1m,
        Type = type,
        CreatedAt = DateTimeOffset.UtcNow,
        WarehouseNumber = warehouseNumber
    };
}
