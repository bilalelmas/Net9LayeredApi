using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.Application.DTOs.Orders;
using Net9LayeredApi.Application.Services.Interfaces;
using Net9LayeredApi.Domain.Entities;
using Net9LayeredApi.Infrastructure.Persistence;

namespace Net9LayeredApi.Application.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(Guid userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        // User kontrolü
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            throw new InvalidOperationException("Kullanıcı bulunamadı.");

        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("Sipariş en az bir ürün içermelidir.");

        var order = new Order
        {
            UserId = dto.UserId,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        decimal totalPrice = 0;

        foreach (var itemDto in dto.Items)
        {
            // Product kontrolü ve stock kontrolü
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Ürün bulunamadı: {itemDto.ProductId}");

            if (product.Stock < itemDto.Quantity)
                throw new InvalidOperationException($"Yetersiz stok: {product.Name} (Mevcut: {product.Stock}, İstenen: {itemDto.Quantity})");

            if (itemDto.Quantity <= 0)
                throw new ArgumentException("Miktar 0'dan büyük olmalıdır.");

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            };

            totalPrice += orderItem.UnitPrice * orderItem.Quantity;
            order.Items.Add(orderItem);

            // Stock güncelleme
            product.Stock -= itemDto.Quantity;
        }

        order.TotalPrice = totalPrice;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto?> UpdateAsync(Guid id, UpdateOrderDto dto)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return null;

        // Status kontrolü
        if (dto.Status != null)
        {
            var validStatuses = new[] { OrderStatus.Pending, OrderStatus.Completed, OrderStatus.Cancelled };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Geçersiz status: {dto.Status}");

            order.Status = dto.Status;
        }

        await _context.SaveChangesAsync();

        // Include ile tekrar çek
        var updatedOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return updatedOrder == null ? null : _mapper.Map<OrderResponseDto>(updatedOrder);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }
}

