using Domain.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Infrastructure.Helpers;

namespace Infrastructure.DAO
{
    public class ProductDAO : IProductDAO
    {
        private readonly IPostgresConnectionFactory _connectionFactory;

        public ProductDAO(IPostgresConnectionFactory postgresConnectionFactory)
        {
            _connectionFactory = postgresConnectionFactory;
        }

        public async Task AddProductAsync(Product product)
        {
            await using var connection = _connectionFactory.GetConnection();

            var sql = @"INSERT INTO product_model
                            (id, name, price, weight, type, created_at, warehouse_number)
                        VALUES
                            (@Id, @Name, @Price, @Weight, @Type, @CreatedAt, @WarehouseNumber)";

            await connection.ExecuteAsync(sql, new
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Weight = product.Weight,
                Type = product.Type.ToString(),
                CreatedAt = product.CreatedAt,
                WarehouseNumber = product.WarehouseNumber
            });
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            await using var connection = _connectionFactory.GetConnection();

            var sql = @"SELECT id, name, price, weight, type, created_at, warehouse_number
                        FROM product_model
                        WHERE id = @id";

            var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { id });

            return product;
        }

        public async Task<Product[]> GetProductsAsync(DateTimeOffset? createdAt, ProductType? type, int? warehouseNumber, int page, int pageSize)
        {
            await using var connection = _connectionFactory.GetConnection();

            var sql = $@"SELECT id, name, price, weight, type, created_at, warehouse_number
                        FROM product_model
                        WHERE
                            (@CreatedAt IS NULL OR created_at >= @CreatedAt)
                            AND (@Type IS NULL OR type = @Type)
                            AND (@WarehouseNumber IS NULL OR warehouse_number = @WarehouseNumber)
                        ORDER BY created_at DESC
                        OFFSET @Offset
                        LIMIT @Limit";

            var products = await connection.QueryAsync<Product>(sql, new
            {
                CreatedAt = createdAt,
                Type = type is null ? null : type.ToString(),
                WarehouseNumber = warehouseNumber,
                Offset = (page - 1) * pageSize,
                Limit = pageSize
            });

            return products.ToArray();
        }

        public async Task<Product?> UpdateProductAsync(Guid id, string? newName, decimal? newPrice, decimal? newWeight, ProductType? newType, int? newWarehouseNumber)
        {
            await using var connection = _connectionFactory.GetConnection();

            var sql = @"UPDATE product_model
                        SET
                            name = COALESCE(@NewName, name),
                            price = COALESCE(@NewPrice, price),
                            weight = COALESCE(@NewWeight, weight),
                            type = COALESCE(@NewType, type),
                            warehouse_number = COALESCE(@NewWarehouseNumber, warehouse_number)
                        WHERE id = @Id
                        RETURNING id, name, price, weight, type, created_at, warehouse_number";

            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new
            {
                Id = id,
                NewName = newName,
                NewPrice = newPrice,
                NewWeight = newWeight,
                NewType = newType is null ? null : newType.ToString(),
                NewWarehouseNumber = newWarehouseNumber
            });
        }
    }
}
