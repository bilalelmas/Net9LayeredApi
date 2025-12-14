using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.Application.DTOs.Users;
using Net9LayeredApi.Application.Exceptions;
using Net9LayeredApi.Application.Services.Interfaces;
using Net9LayeredApi.Domain.Entities;
using Net9LayeredApi.Infrastructure.Persistence;

namespace Net9LayeredApi.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : _mapper.Map<UserResponseDto>(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        // Unique kontrolü - 409 Conflict için özel exception
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new DuplicateException("Bu email adresi zaten kullanılıyor.");

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new DuplicateException("Bu kullanıcı adı zaten kullanılıyor.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        // Email unique kontrolü
        if (dto.Email != null && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new DuplicateException("Bu email adresi zaten kullanılıyor.");
        }

        // Username unique kontrolü
        if (dto.Username != null && dto.Username != user.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new DuplicateException("Bu kullanıcı adı zaten kullanılıyor.");
        }

        _mapper.Map(dto, user);

        // Password güncelleme
        if (dto.Password != null)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }
}

