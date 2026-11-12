using Domain.Models;

namespace Infrastructure.DAO
{
    public interface IProductDAO
    {
        Task AddProductAsync(Product product);

        Task<Product?> GetProductByIdAsync(Guid id);

        Task<Product[]> GetProductsAsync(DateTimeOffset? createdAt, ProductType? type, int? warehouseNumber, int page, int pageSize);

        Task<Product?> UpdateProductAsync(Guid id, string? newName, decimal? newPrice, decimal? newWeight, ProductType? newType, int? newWarehouseNumber);
    }
}
