using UKHO.ShopFacade.Common.Models.Authentication;

namespace UKHO.ShopFacade.Common.Authentication
{
    public interface ICacheProvider
    {
        void AddAuthTokenToCache(string key, AccessTokenItem accessTokenItem, int expiryMinutes);

        AccessTokenItem? GetAuthTokenFromCache(string key);
    }
}
