using Elastic.Clients.Elasticsearch;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8.Index.Shop;

public class ShopDocument
{
    public int? ShopId { get; set; }
    public string? Name { get; set; }
    public decimal? Revenue { get; set; }
    public GeoLocation? Location { get; set; }
}