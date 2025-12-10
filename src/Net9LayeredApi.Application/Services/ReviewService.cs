using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.Application.DTOs.Reviews;
using Net9LayeredApi.Application.Services.Interfaces;
using Net9LayeredApi.Domain.Entities;
using Net9LayeredApi.Infrastructure.Persistence;

namespace Net9LayeredApi.Application.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReviewResponseDto?> GetByIdAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        return review == null ? null : _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetAllAsync()
    {
        var reviews = await _context.Reviews.ToListAsync();
        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetByProductIdAsync(Guid productId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ProductId == productId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<ReviewResponseDto>>(reviews);
    }

    public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto)
    {
        // User ve Product kontrolü
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            throw new InvalidOperationException("Kullanıcı bulunamadı.");

        var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
            throw new InvalidOperationException("Ürün bulunamadı.");

        // Rating kontrolü (1-5 arası)
        if (dto.Rating < 1 || dto.Rating > 5)
            throw new ArgumentException("Rating 1 ile 5 arasında olmalıdır.");

        var review = _mapper.Map<Review>(dto);
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<ReviewResponseDto?> UpdateAsync(Guid id, UpdateReviewDto dto)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return null;

        // Rating kontrolü
        if (dto.Rating.HasValue && (dto.Rating.Value < 1 || dto.Rating.Value > 5))
            throw new ArgumentException("Rating 1 ile 5 arasında olmalıdır.");

        _mapper.Map(dto, review);
        await _context.SaveChangesAsync();

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return true;
    }
}

