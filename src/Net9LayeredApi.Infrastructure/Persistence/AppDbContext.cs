using Microsoft.EntityFrameworkCore;
using Net9LayeredApi.Domain.Entities;
using Net9LayeredApi.Domain.Entities.Base;

namespace Net9LayeredApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configuration'ları doğrudan burada yapılandırılıyor
        // Alternatif: Configuration dosyaları kullanılabilir (IEntityTypeConfiguration<T>)
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        ConfigureEntities(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
    
    private void ConfigureEntities(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired().HasMaxLength(100);
            e.Property(x => x.Email).IsRequired().HasMaxLength(256);
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).IsRequired().HasMaxLength(50);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.Username).IsUnique();
            e.HasMany(x => x.Products).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Orders).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Reviews).WithOne(x => x.User).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        
        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,2)").IsRequired();
            e.Property(x => x.Stock).IsRequired();
            e.HasCheckConstraint("CK_Product_Stock_NonNegative", "[Stock] >= 0");
            e.HasCheckConstraint("CK_Product_Price_NonNegative", "[Price] >= 0");
            e.HasMany(x => x.Reviews).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.OrderItems).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });
        
        // Review
        modelBuilder.Entity<Review>(e =>
        {
            e.ToTable("Reviews");
            e.HasKey(x => x.Id);
            e.Property(x => x.Rating).IsRequired();
            e.Property(x => x.Comment).IsRequired();
            e.HasCheckConstraint("CK_Review_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");
        });
        
        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).IsRequired().HasMaxLength(50).HasDefaultValue(OrderStatus.Pending);
            e.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
            e.HasCheckConstraint("CK_Order_TotalPrice_NonNegative", "[TotalPrice] >= 0");
            e.HasCheckConstraint("CK_Order_Status_Valid", $"[Status] IN ('{OrderStatus.Pending}', '{OrderStatus.Completed}', '{OrderStatus.Cancelled}')");
            e.HasMany(x => x.Items).WithOne(x => x.Order).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        });
        
        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("OrderItems");
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).IsRequired();
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            e.HasCheckConstraint("CK_OrderItem_Quantity_Positive", "[Quantity] > 0");
            e.HasCheckConstraint("CK_OrderItem_UnitPrice_NonNegative", "[UnitPrice] >= 0");
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

