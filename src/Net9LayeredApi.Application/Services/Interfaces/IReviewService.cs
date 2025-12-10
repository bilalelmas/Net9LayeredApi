using Net9LayeredApi.Application.DTOs.Reviews;

namespace Net9LayeredApi.Application.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ReviewResponseDto>> GetAllAsync();
    Task<IEnumerable<ReviewResponseDto>> GetByProductIdAsync(Guid productId);
    Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto);
    Task<ReviewResponseDto?> UpdateAsync(Guid id, UpdateReviewDto dto);
    Task<bool> DeleteAsync(Guid id);
}

