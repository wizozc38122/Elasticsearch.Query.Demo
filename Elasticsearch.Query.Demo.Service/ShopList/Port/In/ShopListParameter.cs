namespace Elasticsearch.Query.Demo.Service.ShopList.Port.In;

public class ShopListParameter
{
    public int? ShopId { get; set; }
    
    public string? Name { get; set; }

    public decimal? MinRevenue { get; set; }
    
    public decimal? MaxRevenue { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int? Distance { get; set; }

    public ShopListSortEnum Sort { get; set; }

    public int? Page { get; set; }

    public int? Size { get; set; }
}