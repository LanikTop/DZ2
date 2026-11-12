using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class UpdateProductRequestDTO
    {
        public Guid Id { get; set; }
        public string? NewName { get; set; }
        public decimal? NewPrice { get; set; }
        public decimal? NewWeight { get; set; }
        public ProductType? NewType { get; set; }
        public int? NewWarehouseNumber { get; set; }
    }
}
