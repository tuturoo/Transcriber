using Core.Shared.Interfaces;
using Core.Shared.Models;

namespace Core.Shared.Abstractions
{
    public abstract class CachedFactory<T> : IFactory<T> where T : class
    {
        private readonly SemaphoreSlim _semaphore;
        private T? _cachedValue;

        public CachedFactory()
        {
            _semaphore = new SemaphoreSlim(1);
            
            _cachedValue = default;
        }

        public virtual async Task<T> CreateAsync(AudioFormat audioFormat, CancellationToken token)
        {
            if (_cachedValue is null)
            {
                try
                {
                    await _semaphore.WaitAsync(token);

                    if (_cachedValue is not null)
                        return _cachedValue;

                    _cachedValue = await InternalCreateAsync(audioFormat, token);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _cachedValue;
        }

        protected abstract Task<T> InternalCreateAsync(AudioFormat audioFormat, CancellationToken token);
    }
}
