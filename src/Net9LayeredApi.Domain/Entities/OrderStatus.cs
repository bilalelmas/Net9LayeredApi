namespace Net9LayeredApi.Domain.Entities;

public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Pending, Completed, Cancelled];
}

