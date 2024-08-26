namespace Elasticsearch.Query.Demo.Service.ShopList.Port.In;

public interface IShopListService
{
    Task<List<ShopListDto>> GetAsync(ShopListParameter parameter);   
}