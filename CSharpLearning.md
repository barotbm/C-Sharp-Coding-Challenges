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
