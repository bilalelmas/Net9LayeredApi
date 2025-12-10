using Net9LayeredApi.Application.DTOs.Products;

namespace Net9LayeredApi.Application.Services.Interfaces;

public interface IProductService
{
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
    Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<bool> DeleteAsync(Guid id);
}

