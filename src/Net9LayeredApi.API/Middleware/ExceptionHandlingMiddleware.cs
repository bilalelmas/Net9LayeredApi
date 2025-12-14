using System.Net;
using System.Text.Json;
using Net9LayeredApi.API.Common;
using Net9LayeredApi.Application.Exceptions;

namespace Net9LayeredApi.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DuplicateException ex)
        {
            _logger.LogWarning(ex, "Çakışma hatası: {Message}", ex.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt başlatıldıktan sonra hata oluştu, status/body yazılamıyor.");
                throw;
            }

            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail(ex.Message);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "İşlem hatası: {Message}", ex.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt başlatıldıktan sonra hata oluştu, status/body yazılamıyor.");
                throw;
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail(ex.Message);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validasyon hatası: {Message}", ex.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt başlatıldıktan sonra hata oluştu, status/body yazılamıyor.");
                throw;
            }

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail(ex.Message);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu: {Message}", ex.Message);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt başlatıldıktan sonra hata oluştu, status/body yazılamıyor.");
                throw;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Development ortamında detaylı hata mesajı göster
            var errorMessage = _environment.IsDevelopment() 
                ? $"Beklenmeyen bir hata oluştu: {ex.Message}" 
                : "Beklenmeyen bir hata oluştu.";
            
            var response = ApiResponse.Fail(errorMessage);
            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}

