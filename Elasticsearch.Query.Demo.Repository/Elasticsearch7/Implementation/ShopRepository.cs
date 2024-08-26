using Elasticsearch.Query.Demo.Repository.Elasticsearch7.Index.Shop;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using MapsterMapper;
using Nest;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch7.Implementation;

public class ShopListRepository(DemoElasticsearch demoElasticsearch, IMapper mapper) : IShopListPort
{
    private readonly Base.ElasticSearchIndex<ShopDocument> _shopIndex = demoElasticsearch.Shop; 

    public async Task<List<ShopListDataModel>> GetShopListAsync(ShopListOutPortParameter outPortParameter)
    {
        var request = new SearchRequest(_shopIndex.IndexName)
        {
            Query = new BoolQuery { Filter = _shopIndex.QuerySetting?.GetQuery(outPortParameter) },
            From = (outPortParameter.Page - 1) * outPortParameter.Size,
            Size = outPortParameter.Size,
            Sort = _shopIndex.QuerySetting?.GetSort(outPortParameter.Sort, outPortParameter)
        };

        var response = await _shopIndex.SearchAsync(request);
        if (!response.IsValid)
        {
            throw new Exception(response.OriginalException?.Message);
        }

        return response.Documents.Select(x =>
        {
            var dataModel = mapper.Map<ShopListDataModel>(x);
            dataModel.Longitude = x.Location?.Longitude;
            dataModel.Latitude = x.Location?.Latitude;
            return dataModel;
        }).ToList();
    }
}