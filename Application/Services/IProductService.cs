using Domain.Models;
using Application.DTOs;

namespace Application.Services;

public interface IProductService
{
    Task<ProductDTO?> GetProductByIdAsync(Guid id);

    Task<CreateProductResponseDTO> CreateProductAsync(CreateProductRequestDTO request);

    Task<GetProductsResponseDTO> GetProductsAsync(GetProductsRequestDTO request);

    Task<ProductDTO?> UpdateProductAsync(UpdateProductRequestDTO request);
}