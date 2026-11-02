using Domain.Models;

namespace Application.DTOs
{
    public class CreateProductRequestDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public ProductType Type { get; set; }
        public int WarehouseNumber { get; set; }
    }
}
