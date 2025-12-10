using Net9LayeredApi.Domain.Entities.Base;

namespace Net9LayeredApi.Domain.Entities;

public class Product : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public User User { get; set; } = default!;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

