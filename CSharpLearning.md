# C# Learning Notes

## Lazy<T>

`Lazy<T>` defers creation of a value until it is needed. It also ensures the value is created only once in a thread-safe way when configured.

### Example

```csharp
using System;
using System.Threading;

public static class LazyExample
{
    private static readonly Lazy<string> _value = new Lazy<string>(
        () =>
        {
            Console.WriteLine("Value factory executed.");
            Thread.Sleep(200);
            return "Hello from Lazy";
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static void Run()
    {
        Console.WriteLine("Before Value");
        Console.WriteLine(_value.Value);
        Console.WriteLine(_value.Value);
        Console.WriteLine("After Value");
    }
}
```

**What you will see:**
- The factory message prints once.
- The value is reused on the second access.

### Simple analogy

Ordering a custom cake:
- You place the order (create `Lazy<T>`), but the bakery doesn’t bake until needed.
- The first person who asks triggers the baking.
- Everyone else gets slices from the same cake.

## LINQ trap: `Count` for “more than N”

If you only need to know **whether there are more than $N$ matches**, `Count` is a poor fit for large sequences because it **must scan the entire sequence**.

### Prefer early-exit logic

```csharp
public static bool HasMoreThan10Active(IEnumerable<LinqTrap.Order> orders)
{
    if (orders is null)
        throw new ArgumentNullException(nameof(orders));

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
```

### Why not `Count`?

- `Count` evaluates every element even after you’ve already found enough matches.
- For huge sequences, the extra work is significant.
- The loop short-circuits on the 11th match.
