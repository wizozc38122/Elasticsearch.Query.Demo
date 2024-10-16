using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Core.Search;

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
    
    public async Task<(IEnumerable<TDocumentModel> Documents, AggregateDictionary? Aggregations)> SearchAfterAsync(
    SearchAfterParameter searchAfterParameter, SearchRequest defaultRequest,
    CancellationToken cancellationToken = default(CancellationToken))
{
    if (searchAfterParameter.Limit > 10000) throw new Exception("Limit 不能大於 10000");
    if (defaultRequest.Sort == null || !defaultRequest.Sort.Any()) throw new Exception("Sort 不能為空");
    if (searchAfterParameter.OrderIdentifier is null) throw new Exception("OrderIdentifier 不能為空");

    // Add Sort Field
    defaultRequest.Sort.Add(SortOptions.Field(
        searchAfterParameter.OrderIdentifier,
        new FieldSort { Order = SortOrder.Asc }));
    
    var querySize = searchAfterParameter.Size * searchAfterParameter.Page;
    var limit = searchAfterParameter.Limit;
    if (querySize <= searchAfterParameter.Limit)
    {
        defaultRequest.From = searchAfterParameter.Size * (searchAfterParameter.Page - 1);
        defaultRequest.Size = searchAfterParameter.Size;
        var searchResponse = await ElasticClient.SearchAsync<TDocumentModel>(defaultRequest, cancellationToken);
        if (!searchResponse.IsValidResponse) throw new Exception(searchResponse.DebugInformation);
        return (searchResponse.Documents, searchResponse.Aggregations);
    }


    // First Query
    var firstResponse = await ElasticClient.SearchAsync<TDocumentModel>(new SearchRequest(IndexName)
    {
        Query = defaultRequest.Query,
        From = limit - 1,
        Size = 1,
        Aggregations = defaultRequest.Aggregations,
        Sort = defaultRequest.Sort,
        TrackTotalHits = new TrackHits(true)
    }, cancellationToken);
    if (!firstResponse.IsValidResponse) throw new Exception(firstResponse.DebugInformation);

    if (querySize > firstResponse.HitsMetadata.Total?.Match(x => x.Value, l => l))
    {
        return (Enumerable.Empty<TDocumentModel>(), firstResponse.Aggregations);
    }

    // Search After
    var loopCount = querySize % limit > 0 ? querySize / limit : querySize / limit - 1;
    var lastSize = querySize % limit > 0 ? querySize % limit : limit;
    var afterKey = firstResponse.Hits.Last().Sort;
    while (loopCount != 1)
    {
        var afterRequest = new SearchRequest(IndexName)
        {
            Query = defaultRequest.Query,
            From = 0,
            Size = limit,
            Sort = defaultRequest.Sort,
            SearchAfter = afterKey?.ToList()
        };
        var afterSearchResponse = await ElasticClient.SearchAsync<TDocumentModel>(afterRequest, cancellationToken);
        if (!afterSearchResponse.IsValidResponse) throw new Exception(afterSearchResponse.DebugInformation);
        afterKey = afterSearchResponse.Hits.Last().Sort;
        loopCount -= 1;
    }

    var lastResponse = await ElasticClient.SearchAsync<TDocumentModel>(new SearchRequest(IndexName)
    {
        Query = defaultRequest.Query,
        From = 0,
        Size = lastSize,
        Sort = defaultRequest.Sort,
        SearchAfter = afterKey?.ToList()
    }, cancellationToken);
    if (!lastResponse.IsValidResponse) throw new Exception(lastResponse.DebugInformation);

    return (lastResponse.Documents
            .Skip(lastSize - searchAfterParameter.Size)
            .Take(searchAfterParameter.Size),
        firstResponse.Aggregations);
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

public class SearchAfterParameter
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int Limit { get; set; }
    public Field OrderIdentifier { get; set; }
}