namespace UKHO.ShopFacade.Common.Authentication
{
    public interface IAuthTokenProvider
    {
        Task<string> GetManagedIdentityAuthAsync(string resource, string scope);
    }
}
