using Domain.Models;

namespace Infrastructure.DAO
{
    public interface IProductDAO
    {
        Task AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(Guid id);

        Task<Product[]> GetProductsAsync(DateTimeOffset? createdAt, ProductType? type, int? warehouseNumber);
    }
}
