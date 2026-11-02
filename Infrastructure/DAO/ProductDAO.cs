using Domain.Models;
using System.Collections.Concurrent;

namespace Infrastructure.DAO
{
    public class ProductDAO : IProductDAO
    {
        private readonly ConcurrentDictionary<Guid, Product> _products = new();

        public Task AddProductAsync(Product product)
        {
            if (!_products.TryAdd(product.Id, product))
            {
                throw new InvalidOperationException(
                    $"Товар с ID {product.Id} уже существует.");
            }

            return Task.CompletedTask;
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            _products.TryGetValue(id, out var product);
            return await Task.FromResult(product);
        }

        public async Task<Product[]> GetProductsAsync(DateTimeOffset? createdAt, ProductType? type, int? warehouseNumber)
        {
            var query = _products.Values.AsEnumerable();
            if (createdAt.HasValue)
                query = query.Where(p => p.CreatedAt >= createdAt.Value);
            if (type.HasValue)
                query = query.Where(p => p.Type == type.Value);
            if (warehouseNumber.HasValue)
                query = query.Where(p => p.WarehouseNumber == warehouseNumber.Value);

            var products = query
                .OrderByDescending(p => p.CreatedAt)
                .ToArray();

            return await Task.FromResult(products);
        }
    }
}
