using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8.Base;

public class ElasticSearchIndex<TDocumentModel> where TDocumentModel : class
{
    public ElasticsearchClient ElasticClient { get; }
    public string IndexName { get; }
    public QuerySettingBase<TDocumentModel>? QuerySetting { get; }

    public Field Field<TFieldType>(Expression<Func<TDocumentModel, TFieldType>> fieldSelector) =>
        new(fieldSelector);

    public Task<SearchResponse<TDocumentModel>> SearchAsync(
        SearchRequest request,  CancellationToken cancellationToken = default(CancellationToken))
    {
        return ElasticClient.SearchAsync<TDocumentModel>(request, cancellationToken);
    }

    public ElasticSearchIndex(ElasticsearchClient elasticClient, string indexName)
    {
        ElasticClient = elasticClient;
        IndexName = indexName;
    }

    public ElasticSearchIndex(ElasticsearchClient elasticClient, string indexName,
        QuerySettingBase<TDocumentModel> querySetting)
    {
        ElasticClient = elasticClient;
        IndexName = indexName;
        QuerySetting = querySetting;
    }
}