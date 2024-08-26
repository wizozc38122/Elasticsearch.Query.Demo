using Elasticsearch.Query.Demo.Repository.Elasticsearch7.Base;
using Elasticsearch.Query.Demo.Repository.Elasticsearch7.Index.Shop;
using Nest;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch7;

/// <summary>
/// Demo Elasticsearch
/// </summary>
/// <param name="elasticClient"></param>
public class DemoElasticsearch(IElasticClient elasticClient) : ElasticSearchBase(elasticClient)
{
    /// <summary>
    /// Shop Index
    /// </summary>
    public readonly ElasticSearchIndex<ShopDocument> Shop = new(elasticClient, "shop", new ShopQuerySetting());
}