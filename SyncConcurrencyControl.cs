using System.Threading;

namespace CodingChallenges;

public sealed class SyncConcurrencyControl
{
    private readonly SemaphoreSlim _semaphore;

    public SyncConcurrencyControl(int maxConcurrency)
    {
        if (maxConcurrency <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxConcurrency), "Max concurrency must be greater than zero.");

        _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> requestFunc, CancellationToken cancellationToken = default)
    {
        if (requestFunc is null)
            throw new ArgumentNullException(nameof(requestFunc));

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await requestFunc().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ExecuteAsync(Func<Task> requestFunc, CancellationToken cancellationToken = default)
    {
        if (requestFunc is null)
            throw new ArgumentNullException(nameof(requestFunc));

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await requestFunc().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
