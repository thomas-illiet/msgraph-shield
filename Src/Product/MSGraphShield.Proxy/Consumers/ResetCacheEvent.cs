using MSGraphShield.Proxy.Plumbings.Cache;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MSGraphShield.Proxy.Events
{
    internal record ResetCacheEvent;

    internal class ResetCacheEventDefinition
    {
        private IInternalCache _memoryCache;

        public Task Consume(ConsumeContext<ResetCacheEvent> context)
        {
            _memoryCache.Reset();
            return Task.CompletedTask;
        }
    }
}
