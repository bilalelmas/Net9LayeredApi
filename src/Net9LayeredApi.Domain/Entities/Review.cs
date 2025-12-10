using Net9LayeredApi.Domain.Entities.Base;

namespace Net9LayeredApi.Domain.Entities;

public class Review : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = default!;

    public User User { get; set; } = default!;
    public Product Product { get; set; } = default!;
}

