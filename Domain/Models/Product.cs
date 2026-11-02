using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Product
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal Weight { get; init; }
        public ProductType Type { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public int WarehouseNumber { get; init; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductType
    {
        General,
        HouseholdChemicals,
        Electronics,
        Food
    }
}
