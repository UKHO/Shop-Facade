using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKHO.ShopFacade.Common.Events;
using UKHO.ShopFacade.Common.Models.Authentication;

namespace UKHO.ShopFacade.Common.Authentication
{
    public class CacheProvider : ICacheProvider
    {
        private readonly ILogger<AuthTokenProvider> _logger;
        private static readonly object s_lock = new();
        private readonly IDistributedCache _cache;

        public CacheProvider(IDistributedCache cache, ILogger<AuthTokenProvider> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public void AddAuthTokenToCache(string key, AccessTokenItem accessTokenItem, int expiryMinutes)
        {
            var tokenExpiryMinutes = accessTokenItem.ExpiresIn.Subtract(DateTime.UtcNow).TotalMinutes;
            var deductTokenExpiryMinutes = expiryMinutes < tokenExpiryMinutes ? expiryMinutes : 1;
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(tokenExpiryMinutes - deductTokenExpiryMinutes));
            options.SetAbsoluteExpiration(accessTokenItem.ExpiresIn);

            lock (s_lock)
            {
                _cache.SetString(key, JsonConvert.SerializeObject(accessTokenItem), options);
                _logger.LogInformation(EventIds.CachingExternalEndPointToken.ToEventId(), "Caching new token for external end point resource {resource} and expires in {ExpiresIn} with sliding expiration duration {options}.", key, Convert.ToString(accessTokenItem.ExpiresIn), JsonConvert.SerializeObject(options));
            }
        }

        public AccessTokenItem? GetAuthTokenFromCache(string key)
        {
            var item = _cache.GetString(key);
            if (item != null)
            {
                return JsonConvert.DeserializeObject<AccessTokenItem>(item);
            }

            return null;
        }
    }
}
