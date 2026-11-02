namespace Application.DTOs
{
    public class GetProductsResponseDTO
    {
        public int TotalCount { get; set; }
        public ProductDTO[] Products { get; set; }
    }
}
