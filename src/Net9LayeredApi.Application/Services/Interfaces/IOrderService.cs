using Net9LayeredApi.Application.DTOs.Orders;

namespace Net9LayeredApi.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderResponseDto>> GetAllAsync();
    Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(Guid userId);
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    Task<OrderResponseDto?> UpdateAsync(Guid id, UpdateOrderDto dto);
    Task<bool> DeleteAsync(Guid id);
}

