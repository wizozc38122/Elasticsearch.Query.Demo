using Elasticsearch.Query.Demo.Service.ShopList.Port.In;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using MapsterMapper;

namespace Elasticsearch.Query.Demo.Service.ShopList;

public class ShopListService(IShopListPort shopListPort, IMapper mapper) : IShopListService
{
    public async Task<List<ShopListDto>> GetAsync(ShopListParameter parameter)
    {
        // Parameter validation
        if (parameter.Size is null or <= 0) parameter.Size = 20;
        if (parameter.Page is null or <= 0) parameter.Page = 1;

        // Get
        var result = await shopListPort.GetShopListAsync(mapper.Map<ShopListOutPortParameter>(parameter));

        // TODO: Implement the logic here

        return mapper.Map<List<ShopListDto>>(result);
    }
}