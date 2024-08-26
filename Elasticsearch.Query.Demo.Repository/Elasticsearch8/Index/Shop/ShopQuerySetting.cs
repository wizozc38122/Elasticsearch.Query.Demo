using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Query.Demo.Repository.Elasticsearch8.Base;
using Elasticsearch.Query.Demo.Service.ShopList.Port;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using DistanceUnit = Elastic.Clients.Elasticsearch.DistanceUnit;
using FieldSort = Elastic.Clients.Elasticsearch.FieldSort;
using GeoDistanceQuery = Elastic.Clients.Elasticsearch.QueryDsl.GeoDistanceQuery;
using GeoDistanceSort = Elastic.Clients.Elasticsearch.GeoDistanceSort;
using GeoDistanceType = Elastic.Clients.Elasticsearch.GeoDistanceType;
using GeoLocation = Elastic.Clients.Elasticsearch.GeoLocation;
using SortOrder = Elastic.Clients.Elasticsearch.SortOrder;
using SuffixExtensions = Nest.SuffixExtensions;
using TermQuery = Elastic.Clients.Elasticsearch.QueryDsl.TermQuery;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8.Index.Shop;

public class ShopQuerySetting : QuerySettingBase<ShopDocument>
{
    public ShopQuerySetting()
    {
        SetShopListOutPortParameter();
    }

    private void SetShopListOutPortParameter()
    {
        Set<ShopListOutPortParameter>()
            .SetQuery(p =>
            {
                if (p.ShopId is null) return null;
                return new TermQuery(Field(f => f.ShopId))
                {
                    Value = p.ShopId!.Value
                };
            })
            .SetQueryWhenAllNotNull([p => p.Name],
                p => new TermQuery(Field(f => SuffixExtensions.Suffix(f.Name, "keyword")))
                {
                    Value = p.Name!
                })
            .SetQueryWhenAllNotNull([p => p.MinRevenue],
                p => new NumberRangeQuery(Field(f => f.Revenue))
                {
                    Gte = (double)p.MinRevenue!.Value
                })
            .SetQueryWhenAllNotNull([p => p.MaxRevenue],
                p => new NumberRangeQuery(Field(f => f.Revenue))
                {
                    Lte = (double)p.MaxRevenue!.Value
                })
            .SetQueryWhenAllNotNull([p => p.Latitude, p => p.Longitude, p => p.Distance],
                p => new GeoDistanceQuery
                {
                    Field = Field(f => f.Location),
                    DistanceType = GeoDistanceType.Arc,
                    Location = GeoLocation.LatitudeLongitude(new LatLonGeoLocation
                    {
                        Lat = p.Latitude!.Value,
                        Lon = p.Longitude!.Value
                    }),
                    Distance = $"{p.Distance}m"
                })
            .SetSort<ShopListSortEnum>()
            .Set(ShopListSortEnum.RevenueAsc, s =>
            [
                SortOptions.Field(Field(f => f.Revenue), new FieldSort { Order = SortOrder.Asc })
            ])
            .Set(ShopListSortEnum.RevenueDesc, s =>
            [
                SortOptions.Field(Field(f => f.Revenue), new FieldSort { Order = SortOrder.Desc })
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
                            Order = SortOrder.Asc,
                            DistanceType = GeoDistanceType.Arc,
                            Unit = DistanceUnit.Meters,
                            Location =
                            [
                                GeoLocation.LatitudeLongitude(new LatLonGeoLocation
                                {
                                    Lat = s.Latitude!.Value,
                                    Lon = s.Longitude!.Value
                                })
                            ]
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
                            Order = SortOrder.Desc,
                            DistanceType = GeoDistanceType.Arc,
                            Unit = DistanceUnit.Meters,
                            Location =
                            [
                                GeoLocation.LatitudeLongitude(new LatLonGeoLocation
                                {
                                    Lat = s.Latitude!.Value,
                                    Lon = s.Longitude!.Value
                                })
                            ]
                        }
                    ];
                }

                return [];
            });
    }
}