using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.API.Common;
using Net9LayeredApi.API.Middleware;
using Net9LayeredApi.Application.DTOs.Orders;
using Net9LayeredApi.Application.DTOs.Products;
using Net9LayeredApi.Application.DTOs.Reviews;
using Net9LayeredApi.Application.DTOs.Users;
using Net9LayeredApi.Application.Mapping;
using Net9LayeredApi.Application.Services;
using Net9LayeredApi.Application.Services.Interfaces;
using Net9LayeredApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Geçici: Veritabanını oluştur (migration yerine)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Veritabanı oluşturuluyor...");
        var created = dbContext.Database.EnsureCreated();
        logger.LogInformation($"Veritabanı oluşturuldu: {created}");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı oluşturulurken hata oluştu");
    }
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Net9 Layered API v1");
        c.RoutePrefix = string.Empty; // Swagger'ı root'ta göster
    });
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// Health check
app.MapGet("/ping", () => "pong");

// Users endpoints
app.MapGet("/api/users", async (IUserService service) =>
{
    var users = await service.GetAllAsync();
    return Results.Ok(ApiResponse<IEnumerable<UserResponseDto>>.Ok(users, "Kullanıcılar başarıyla getirildi."));
})
.WithName("GetAllUsers")
.WithTags("Users")
.Produces<ApiResponse<IEnumerable<UserResponseDto>>>(StatusCodes.Status200OK);

app.MapGet("/api/users/{id:guid}", async (Guid id, IUserService service) =>
{
    var user = await service.GetByIdAsync(id);
    if (user == null)
        return Results.NotFound(ApiResponse<UserResponseDto>.Fail("Kullanıcı bulunamadı."));

    return Results.Ok(ApiResponse<UserResponseDto>.Ok(user, "Kullanıcı başarıyla getirildi."));
})
.WithName("GetUserById")
.WithTags("Users")
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status404NotFound);

app.MapPost("/api/users", async ([FromBody] CreateUserDto dto, IUserService service) =>
{
    try
    {
        var user = await service.CreateAsync(dto);
        return Results.Created($"/api/users/{user.Id}", ApiResponse<UserResponseDto>.Ok(user, "Kullanıcı başarıyla oluşturuldu."));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ApiResponse<UserResponseDto>.Fail(ex.Message));
    }
})
.WithName("CreateUser")
.WithTags("Users")
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status201Created)
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status400BadRequest);

app.MapPut("/api/users/{id:guid}", async (Guid id, [FromBody] UpdateUserDto dto, IUserService service) =>
{
    try
    {
        var user = await service.UpdateAsync(id, dto);
        if (user == null)
            return Results.NotFound(ApiResponse<UserResponseDto>.Fail("Kullanıcı bulunamadı."));

        return Results.Ok(ApiResponse<UserResponseDto>.Ok(user, "Kullanıcı başarıyla güncellendi."));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ApiResponse<UserResponseDto>.Fail(ex.Message));
    }
})
.WithName("UpdateUser")
.WithTags("Users")
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status404NotFound)
.Produces<ApiResponse<UserResponseDto>>(StatusCodes.Status400BadRequest);

app.MapDelete("/api/users/{id:guid}", async (Guid id, IUserService service) =>
{
    var deleted = await service.DeleteAsync(id);
    if (!deleted)
        return Results.NotFound(ApiResponse.Fail("Kullanıcı bulunamadı."));

    return Results.Ok(ApiResponse.Ok("Kullanıcı başarıyla silindi."));
})
.WithName("DeleteUser")
.WithTags("Users")
.Produces<ApiResponse>(StatusCodes.Status200OK)
.Produces<ApiResponse>(StatusCodes.Status404NotFound);

// Products endpoints
app.MapGet("/api/products", async (IProductService service) =>
{
    var products = await service.GetAllAsync();
    return Results.Ok(ApiResponse<IEnumerable<ProductResponseDto>>.Ok(products, "Ürünler başarıyla getirildi."));
})
.WithName("GetAllProducts")
.WithTags("Products")
.Produces<ApiResponse<IEnumerable<ProductResponseDto>>>(StatusCodes.Status200OK);

app.MapGet("/api/products/{id:guid}", async (Guid id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    if (product == null)
        return Results.NotFound(ApiResponse<ProductResponseDto>.Fail("Ürün bulunamadı."));

    return Results.Ok(ApiResponse<ProductResponseDto>.Ok(product, "Ürün başarıyla getirildi."));
})
.WithName("GetProductById")
.WithTags("Products")
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status404NotFound);

app.MapPost("/api/products", async ([FromBody] CreateProductDto dto, IProductService service) =>
{
    try
    {
        var product = await service.CreateAsync(dto);
        return Results.Created($"/api/products/{product.Id}", ApiResponse<ProductResponseDto>.Ok(product, "Ürün başarıyla oluşturuldu."));
    }
    catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
    {
        return Results.BadRequest(ApiResponse<ProductResponseDto>.Fail(ex.Message));
    }
})
.WithName("CreateProduct")
.WithTags("Products")
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status201Created)
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status400BadRequest);

app.MapPut("/api/products/{id:guid}", async (Guid id, [FromBody] UpdateProductDto dto, IProductService service) =>
{
    try
    {
        var product = await service.UpdateAsync(id, dto);
        if (product == null)
            return Results.NotFound(ApiResponse<ProductResponseDto>.Fail("Ürün bulunamadı."));

        return Results.Ok(ApiResponse<ProductResponseDto>.Ok(product, "Ürün başarıyla güncellendi."));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ApiResponse<ProductResponseDto>.Fail(ex.Message));
    }
})
.WithName("UpdateProduct")
.WithTags("Products")
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status404NotFound)
.Produces<ApiResponse<ProductResponseDto>>(StatusCodes.Status400BadRequest);

app.MapDelete("/api/products/{id:guid}", async (Guid id, IProductService service) =>
{
    var deleted = await service.DeleteAsync(id);
    if (!deleted)
        return Results.NotFound(ApiResponse.Fail("Ürün bulunamadı."));

    return Results.Ok(ApiResponse.Ok("Ürün başarıyla silindi."));
})
.WithName("DeleteProduct")
.WithTags("Products")
.Produces<ApiResponse>(StatusCodes.Status200OK)
.Produces<ApiResponse>(StatusCodes.Status404NotFound);

// Reviews endpoints
app.MapGet("/api/reviews", async (IReviewService service) =>
{
    var reviews = await service.GetAllAsync();
    return Results.Ok(ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviews, "Yorumlar başarıyla getirildi."));
})
.WithName("GetAllReviews")
.WithTags("Reviews")
.Produces<ApiResponse<IEnumerable<ReviewResponseDto>>>(StatusCodes.Status200OK);

app.MapGet("/api/reviews/{id:guid}", async (Guid id, IReviewService service) =>
{
    var review = await service.GetByIdAsync(id);
    if (review == null)
        return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail("Yorum bulunamadı."));

    return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Yorum başarıyla getirildi."));
})
.WithName("GetReviewById")
.WithTags("Reviews")
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound);

app.MapGet("/api/products/{productId:guid}/reviews", async (Guid productId, IReviewService service) =>
{
    var reviews = await service.GetByProductIdAsync(productId);
    return Results.Ok(ApiResponse<IEnumerable<ReviewResponseDto>>.Ok(reviews, "Ürün yorumları başarıyla getirildi."));
})
.WithName("GetReviewsByProductId")
.WithTags("Reviews")
.Produces<ApiResponse<IEnumerable<ReviewResponseDto>>>(StatusCodes.Status200OK);

app.MapPost("/api/reviews", async ([FromBody] CreateReviewDto dto, IReviewService service) =>
{
    try
    {
        var review = await service.CreateAsync(dto);
        return Results.Created($"/api/reviews/{review.Id}", ApiResponse<ReviewResponseDto>.Ok(review, "Yorum başarıyla oluşturuldu."));
    }
    catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
    {
        return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
    }
})
.WithName("CreateReview")
.WithTags("Reviews")
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status201Created)
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status400BadRequest);

app.MapPut("/api/reviews/{id:guid}", async (Guid id, [FromBody] UpdateReviewDto dto, IReviewService service) =>
{
    try
    {
        var review = await service.UpdateAsync(id, dto);
        if (review == null)
            return Results.NotFound(ApiResponse<ReviewResponseDto>.Fail("Yorum bulunamadı."));

        return Results.Ok(ApiResponse<ReviewResponseDto>.Ok(review, "Yorum başarıyla güncellendi."));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ApiResponse<ReviewResponseDto>.Fail(ex.Message));
    }
})
.WithName("UpdateReview")
.WithTags("Reviews")
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status404NotFound)
.Produces<ApiResponse<ReviewResponseDto>>(StatusCodes.Status400BadRequest);

app.MapDelete("/api/reviews/{id:guid}", async (Guid id, IReviewService service) =>
{
    var deleted = await service.DeleteAsync(id);
    if (!deleted)
        return Results.NotFound(ApiResponse.Fail("Yorum bulunamadı."));

    return Results.Ok(ApiResponse.Ok("Yorum başarıyla silindi."));
})
.WithName("DeleteReview")
.WithTags("Reviews")
.Produces<ApiResponse>(StatusCodes.Status200OK)
.Produces<ApiResponse>(StatusCodes.Status404NotFound);

// Orders endpoints
app.MapGet("/api/orders", async (IOrderService service) =>
{
    var orders = await service.GetAllAsync();
    return Results.Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders, "Siparişler başarıyla getirildi."));
})
.WithName("GetAllOrders")
.WithTags("Orders")
.Produces<ApiResponse<IEnumerable<OrderResponseDto>>>(StatusCodes.Status200OK);

app.MapGet("/api/orders/{id:guid}", async (Guid id, IOrderService service) =>
{
    var order = await service.GetByIdAsync(id);
    if (order == null)
        return Results.NotFound(ApiResponse<OrderResponseDto>.Fail("Sipariş bulunamadı."));

    return Results.Ok(ApiResponse<OrderResponseDto>.Ok(order, "Sipariş başarıyla getirildi."));
})
.WithName("GetOrderById")
.WithTags("Orders")
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status404NotFound);

app.MapGet("/api/users/{userId:guid}/orders", async (Guid userId, IOrderService service) =>
{
    var orders = await service.GetByUserIdAsync(userId);
    return Results.Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders, "Kullanıcı siparişleri başarıyla getirildi."));
})
.WithName("GetOrdersByUserId")
.WithTags("Orders")
.Produces<ApiResponse<IEnumerable<OrderResponseDto>>>(StatusCodes.Status200OK);

app.MapPost("/api/orders", async ([FromBody] CreateOrderDto dto, IOrderService service) =>
{
    try
    {
        var order = await service.CreateAsync(dto);
        return Results.Created($"/api/orders/{order.Id}", ApiResponse<OrderResponseDto>.Ok(order, "Sipariş başarıyla oluşturuldu."));
    }
    catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
    {
        return Results.BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
    }
})
.WithName("CreateOrder")
.WithTags("Orders")
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status201Created)
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status400BadRequest);

app.MapPut("/api/orders/{id:guid}", async (Guid id, [FromBody] UpdateOrderDto dto, IOrderService service) =>
{
    try
    {
        var order = await service.UpdateAsync(id, dto);
        if (order == null)
            return Results.NotFound(ApiResponse<OrderResponseDto>.Fail("Sipariş bulunamadı."));

        return Results.Ok(ApiResponse<OrderResponseDto>.Ok(order, "Sipariş başarıyla güncellendi."));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ApiResponse<OrderResponseDto>.Fail(ex.Message));
    }
})
.WithName("UpdateOrder")
.WithTags("Orders")
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status200OK)
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status404NotFound)
.Produces<ApiResponse<OrderResponseDto>>(StatusCodes.Status400BadRequest);

app.MapDelete("/api/orders/{id:guid}", async (Guid id, IOrderService service) =>
{
    var deleted = await service.DeleteAsync(id);
    if (!deleted)
        return Results.NotFound(ApiResponse.Fail("Sipariş bulunamadı."));

    return Results.Ok(ApiResponse.Ok("Sipariş başarıyla silindi."));
})
.WithName("DeleteOrder")
.WithTags("Orders")
.Produces<ApiResponse>(StatusCodes.Status200OK)
.Produces<ApiResponse>(StatusCodes.Status404NotFound);

app.Run();
