using Net9LayeredApi.Domain.Entities.Base;

namespace Net9LayeredApi.Domain.Entities;

public class Order : AuditableEntity
{
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = OrderStatus.Pending;

    public User User { get; set; } = default!;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}

