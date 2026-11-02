using Domain.Models;

namespace Application.DTOs
{
    public class GetProductsRequestDTO
    {
        public DateTimeOffset? CreatedAt { get; set; }
        public ProductType? Type { get; set; }
        public int? WarehouseNumber { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
