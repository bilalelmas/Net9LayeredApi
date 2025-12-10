using Net9LayeredApi.Domain.Entities.Base;

namespace Net9LayeredApi.Domain.Entities;

public class OrderItem : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = default!;
    public Product Product { get; set; } = default!;
}

