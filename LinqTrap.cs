namespace CodingChallenges;

public sealed class LinqTrap
{
    public sealed record Order(int Id, bool IsActive);

    // Original (inefficient) example would enumerate multiple times.
    public bool HasActiveOrders(IEnumerable<Order> orders)
    {
        if (orders is null)
            throw new ArgumentNullException(nameof(orders));

        // Manual loop: early-exit after the 11th active order.
        // Why not LINQ Count()? Count will scan the entire sequence, even when you only
        // need to know if there are more than 10 matches.
        var activeCount = 0;
        foreach (var order in orders)
        {
            if (!order.IsActive)
                continue;

            activeCount++;
            if (activeCount > 10)
                return true;
        }

        return false;
    }
}
