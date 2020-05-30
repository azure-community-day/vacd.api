using Microsoft.Extensions.Caching.Memory;
using System;

namespace IAC.AZ.Tools.ServicePrincipalManager.Tests
{
    public class MemoryCacheFixture : IDisposable
    {
        public IMemoryCache Cache { get; }

        public MemoryCacheFixture()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cache.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
