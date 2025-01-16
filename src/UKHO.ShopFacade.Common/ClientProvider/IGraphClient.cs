using Microsoft.Graph.Models;

namespace UKHO.ShopFacade.Common.ClientProvider
{
    public interface IGraphClient
    {
        Task<ListItemCollectionResponse> GetListItemCollectionResponse(string expandFields, string filterCondition);
    }
}
