namespace Elasticsearch.Query.Demo.Service.ShopList.Port.Out;

public interface IShopListPort
{
    Task<List<ShopListDataModel>> GetShopListAsync(ShopListOutPortParameter outPortParameter);
}