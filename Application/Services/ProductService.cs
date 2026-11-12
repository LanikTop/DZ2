using Domain.Models;
using Application.DTOs;
using Infrastructure.DAO;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductDAO _productDAO;

    public ProductService(IProductDAO productDAO)
    {
        _productDAO = productDAO;
    }

    public async Task<CreateProductResponseDTO> CreateProductAsync(CreateProductRequestDTO request)
    {
        var product = new Product { 
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Weight = request.Weight,
            Type = request.Type,
            CreatedAt = DateTimeOffset.UtcNow,
            WarehouseNumber = request.WarehouseNumber };

        await _productDAO.AddProductAsync(product);

        return new CreateProductResponseDTO { Id = product.Id };
    }

    public async Task<ProductDTO?> GetProductByIdAsync(Guid id)
    {
        var product = await _productDAO.GetProductByIdAsync(id);

        if (product == null) return null;

        return new ProductDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Weight = product.Weight,
            Type = product.Type,
            CreatedAt = product.CreatedAt,
            WarehouseNumber = product.WarehouseNumber
        };
    }

    public async Task<GetProductsResponseDTO> GetProductsAsync(GetProductsRequestDTO request)
    {
        var products = await _productDAO.GetProductsAsync(request.CreatedAt, request.Type, request.WarehouseNumber);

        var productDTOs = products
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDTO
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Weight = p.Weight,
            Type = p.Type,
            CreatedAt = p.CreatedAt,
            WarehouseNumber = p.WarehouseNumber
        }).ToArray();

        return new GetProductsResponseDTO { Products = productDTOs, TotalCount = products.Length };
    }

    public async Task<ProductDTO?> UpdateProductAsync(UpdateProductRequestDTO request)
    {
        var product = await _productDAO.UpdateProductAsync(request.Id, request.NewName, request.NewPrice, request.NewWeight, request.NewType, request.NewWarehouseNumber);

        if (product == null) return null;

        return new ProductDTO
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Weight = product.Weight,
            Type = product.Type,
            CreatedAt = product.CreatedAt,
            WarehouseNumber = product.WarehouseNumber
        };
    }
}