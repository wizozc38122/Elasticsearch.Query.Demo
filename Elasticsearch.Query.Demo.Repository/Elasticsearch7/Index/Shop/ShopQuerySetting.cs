using Elasticsearch.Query.Demo.Repository.Elasticsearch7.Base;
using Elasticsearch.Query.Demo.Service.ShopList.Port;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using Nest;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch7.Index.Shop;

public class ShopQuerySetting : QuerySettingBase<ShopDocument>
{
    public ShopQuerySetting()
    {
        SetShopListOutPortParameter();
    }

    private void SetShopListOutPortParameter()
    {
        Set<ShopListOutPortParameter>()
            .SetQueryWhenAllNotNull([p => p.ShopId],
                p => new TermQuery
                {
                    Field = Field(f => f.ShopId),
                    Value = p.ShopId
                }) 
            .SetQueryWhenAllNotNull([p => p.Name],
                p => new TermQuery
                {
                    Field = Field(f => f.Name.Suffix("keyword")),
                    Value = p.Name
                })
            .SetQueryWhenAllNotNull([p => p.MinRevenue],
                p => new NumericRangeQuery
                {
                    Field = Field(f => f.Revenue),
                    GreaterThanOrEqualTo = (double)p.MinRevenue!
                })
            .SetQueryWhenAllNotNull([p => p.MaxRevenue],
                p => new NumericRangeQuery
                {
                    Field = Field(f => f.Revenue),
                    LessThanOrEqualTo = (double)p.MaxRevenue!
                })
            .SetQueryWhenAllNotNull([p => p.Latitude, p => p.Longitude, p => p.Distance],
                p => new GeoDistanceQuery
                {
                    Field = Field(f => f.Location),
                    DistanceType = GeoDistanceType.Arc,
                    Location = new GeoLocation(p.Latitude!.Value, p.Longitude!.Value),
                    Distance = $"{p.Distance}m"
                })
            .SetSort<ShopListSortEnum>()
            .Set(ShopListSortEnum.RevenueAsc, s =>
            [
                new FieldSort
                {
                    Field = Field(f => f.Revenue),
                    Order = SortOrder.Ascending
                }
            ])
            .Set(ShopListSortEnum.RevenueDesc, s =>
            [
                new FieldSort
                {
                    Field = Field(f => f.Revenue),
                    Order = SortOrder.Descending
                }
            ])
            .Set(ShopListSortEnum.DistanceFromCenterAsc, s =>
            {
                if (s is { Latitude: not null, Longitude: not null })
                {
                    return
                    [
                        new GeoDistanceSort
                        {
                            Field = Field(f => f.Location),
                            Order = SortOrder.Ascending,
                            Points = new List<GeoLocation>
                            {
                                new(s.Latitude.Value, s.Longitude.Value)
                            },
                            DistanceType = GeoDistanceType.Arc,
                            Unit = DistanceUnit.Meters
                        }
                    ];
                }

                return [];
            })
            .Set(ShopListSortEnum.DistanceFromCenterDesc, s =>
            {
                if (s is { Latitude: not null, Longitude: not null })
                {
                    return
                    [
                        new GeoDistanceSort
                        {
                            Field = Field(f => f.Location),
                            Order = SortOrder.Descending,
                            Points = new List<GeoLocation>
                            {
                                new(s.Latitude.Value, s.Longitude.Value)
                            },
                            DistanceType = GeoDistanceType.Arc,
                            Unit = DistanceUnit.Meters
                        }
                    ];
                }

                return [];
            });
    }
}