using System.Collections.Concurrent;
using System.Threading;

namespace CodingChallenges;

public sealed class IdempotentReqProcessor
{
    private readonly ConcurrentDictionary<string, Lazy<Task<PaymentResult>>> _idempotencyDictionary =
        new();

    public Task<PaymentResult> ProcessPaymentAsync(
        string idempotencyKey,
        Func<Task<PaymentResult>> processFunc)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("Idempotency key is required.", nameof(idempotencyKey));

        if (processFunc is null)
            throw new ArgumentNullException(nameof(processFunc));
        var lazyTask = _idempotencyDictionary.GetOrAdd(
            idempotencyKey,
            _ => new Lazy<Task<PaymentResult>>(
                () => ExecuteWithEvictionAsync(idempotencyKey, processFunc),
                LazyThreadSafetyMode.ExecutionAndPublication));

        return lazyTask.Value;
    }

    private async Task<PaymentResult> ExecuteWithEvictionAsync(
        string idempotencyKey,
        Func<Task<PaymentResult>> processFunc)
    {
        try
        {
            return await processFunc().ConfigureAwait(false);
        }
        catch
        {
            _idempotencyDictionary.TryRemove(idempotencyKey, out _);
            throw;
        }
    }
}

public static class LazyExample
{
    private static readonly Lazy<string> _value = new Lazy<string>(
        () =>
        {
            var random = Guid.NewGuid();
            Console.WriteLine("Value factory executed.");
            Thread.Sleep(200);
            return "Hello from Lazy" + random;
        },
        LazyThreadSafetyMode.ExecutionAndPublication);

    public static void Run()
    {
        Console.WriteLine("Before Value");
        Console.WriteLine(_value.Value);
        Console.WriteLine(_value.Value);
        
        Console.WriteLine(_value.Value);
        
        Console.WriteLine(_value.Value);
        
        Console.WriteLine(_value.Value);
        Console.WriteLine(_value.Value);
        Console.WriteLine(_value.Value);
        Console.WriteLine("After Value");
    }
}