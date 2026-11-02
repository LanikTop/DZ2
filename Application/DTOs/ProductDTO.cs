using Domain.Models;

namespace Application.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public ProductType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int WarehouseNumber { get; set; }
    }
}
