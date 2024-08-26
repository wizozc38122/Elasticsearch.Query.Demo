using Elastic.Clients.Elasticsearch;
using Elasticsearch.Query.Demo.Repository.Elasticsearch8.Base;
using Elasticsearch.Query.Demo.Repository.Elasticsearch8.Index.Shop;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8;

/// <summary>
/// Demo Elasticsearch
/// </summary>
/// <param name="elasticClient"></param>
public class DemoElasticsearch(ElasticsearchClient elasticClient) : ElasticSearchBase(elasticClient)
{
    /// <summary>
    /// Shop Index
    /// </summary>
    public readonly ElasticSearchIndex<ShopDocument> Shop = new(elasticClient, "shop", new ShopQuerySetting());
}