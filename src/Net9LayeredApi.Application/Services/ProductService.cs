using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.Application.DTOs.Products;
using Net9LayeredApi.Application.Services.Interfaces;
using Net9LayeredApi.Domain.Entities;
using Net9LayeredApi.Infrastructure.Persistence;

namespace Net9LayeredApi.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        return product == null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _context.Products.ToListAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        // User kontrolü
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            throw new InvalidOperationException("Kullanıcı bulunamadı.");

        // Stock ve Price kontrolü
        if (dto.Stock < 0)
            throw new ArgumentException("Stok miktarı negatif olamaz.");

        if (dto.Price < 0)
            throw new ArgumentException("Fiyat negatif olamaz.");

        var product = _mapper.Map<Product>(dto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        // Stock kontrolü
        if (dto.Stock.HasValue && dto.Stock.Value < 0)
            throw new ArgumentException("Stok miktarı negatif olamaz.");

        // Price kontrolü
        if (dto.Price.HasValue && dto.Price.Value < 0)
            throw new ArgumentException("Fiyat negatif olamaz.");

        _mapper.Map(dto, product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }
}

