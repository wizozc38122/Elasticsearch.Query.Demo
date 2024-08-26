namespace Elasticsearch.Query.Demo.Service.ShopList.Port.In;

public class ShopListDto
{
    public int? ShopId { get; set; }

    public string? Name { get; set; }

    public decimal? Revenue { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}