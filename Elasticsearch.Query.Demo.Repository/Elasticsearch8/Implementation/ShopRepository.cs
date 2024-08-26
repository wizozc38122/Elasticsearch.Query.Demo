using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Query.Demo.Repository.Elasticsearch8.Base;
using Elasticsearch.Query.Demo.Repository.Elasticsearch8.Index.Shop;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using MapsterMapper;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8.Implementation;

public class ShopListRepository(DemoElasticsearch demoElasticsearch, IMapper mapper) : IShopListPort
{
    private readonly ElasticSearchIndex<ShopDocument> _shopIndex = demoElasticsearch.Shop;

    public async Task<List<ShopListDataModel>> GetShopListAsync(ShopListOutPortParameter outPortParameter)
    {
        var request = new SearchRequest(_shopIndex.IndexName)
        {
            Query = new BoolQuery { Filter = _shopIndex.QuerySetting?.GetQuery(outPortParameter).ToList() },
            From = (outPortParameter.Page - 1) * outPortParameter.Size,
            Size = outPortParameter.Size,
            Sort = _shopIndex.QuerySetting?.GetSort(outPortParameter.Sort, outPortParameter)
        };

        var response = await _shopIndex.SearchAsync(request);
        
        if (!response.IsValidResponse)
        {
            throw new Exception(response.DebugInformation);
        }

        return response.Documents.Select(x =>
        {
            var dataModel = mapper.Map<ShopListDataModel>(x);
            if (x.Location == null || !x.Location.TryGetLatitudeLongitude(out var latitudeLongitude)) return dataModel;
            dataModel.Longitude = latitudeLongitude.Lon;
            dataModel.Latitude = latitudeLongitude.Lat;
            return dataModel;
        }).ToList();
    }
}