using System.Linq.Expressions;
using Nest;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch7.Base;

public class ElasticSearchIndex<TDocumentModel> where TDocumentModel : class
{
    public IElasticClient ElasticClient { get; }
    public string IndexName { get; }
    public QuerySettingBase<TDocumentModel>? QuerySetting { get; }

    public Field Field<TFieldType>(Expression<Func<TDocumentModel, TFieldType>> fieldSelector) =>
        new(fieldSelector);

    public Task<ISearchResponse<TDocumentModel>> SearchAsync(
        ISearchRequest request,
        CancellationToken ct = default(CancellationToken))
    {
        return ElasticClient.SearchAsync<TDocumentModel>(request, ct);
    }


    public ElasticSearchIndex(IElasticClient elasticClient, string indexName)
    {
        ElasticClient = elasticClient;
        IndexName = indexName;
    }

    public ElasticSearchIndex(IElasticClient elasticClient, string indexName,
        QuerySettingBase<TDocumentModel> querySetting)
    {
        ElasticClient = elasticClient;
        IndexName = indexName;
        QuerySetting = querySetting;
    }
}