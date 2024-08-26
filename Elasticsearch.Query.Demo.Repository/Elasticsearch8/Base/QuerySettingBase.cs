using System.Linq.Expressions;
using Elastic.Clients.Elasticsearch;

namespace Elasticsearch.Query.Demo.Repository.Elasticsearch8.Base;

/// <summary>
/// Query setting base class.
/// </summary>
/// <typeparam name="TDocument">Index Document Model</typeparam>
public abstract class QuerySettingBase<TDocument>
    where TDocument : class
{
    private readonly Dictionary<Type, object> _queryDict = new();

    protected Field Field<TFieldType>(Expression<Func<TDocument, TFieldType>> fieldSelector)
    {
        var customField = new Field(fieldSelector);
        return customField;
    }

    protected QuerySetting<TQueryParameter> Set<TQueryParameter>() where TQueryParameter : class
    {
        var setting = new QuerySetting<TQueryParameter>();
        _queryDict[typeof(TQueryParameter)] = setting;
        return setting;
    }

    public IEnumerable<Elastic.Clients.Elasticsearch.QueryDsl.Query> GetQuery<TQueryParameter>(TQueryParameter query) where TQueryParameter : class
    {
        if (_queryDict.TryGetValue(typeof(TQueryParameter), out var settingObj))
        {
            var setting = (QuerySetting<TQueryParameter>)settingObj;
            return setting.GetQuery(query);
        }

        throw new Exception("No setting is set for the specified type.");
    }

    public IList<SortOptions> GetSort<TSortParameter, TQueryParameter>(TSortParameter sort, TQueryParameter parameter)
        where TSortParameter : Enum
        where TQueryParameter : class
    {
        if (_queryDict.TryGetValue(typeof(TQueryParameter), out var settingObj))
        {
            var setting = (QuerySetting<TQueryParameter>)settingObj;
            if (setting.SortDict.TryGetValue(typeof(TSortParameter), out var sortSettingObj))
            {
                var x = (QuerySetting<TQueryParameter>.SortSetting<TSortParameter>)sortSettingObj;
                return x.GetSort(sort)?.Invoke(parameter) ?? [];
            }
        }

        throw new Exception("No setting is set for the specified type.");
    }

    protected class QuerySetting<TQueryParameter> where TQueryParameter : class
    {
        private readonly List<Func<TQueryParameter, Elastic.Clients.Elasticsearch.QueryDsl.Query?>> _settingList = [];
        protected internal readonly Dictionary<Type, object> SortDict = new();

        internal QuerySetting<TQueryParameter> SetQuery(Func<TQueryParameter, Elastic.Clients.Elasticsearch.QueryDsl.Query?> query)
        {
            _settingList.Add(query);
            return this;
        }

        internal QuerySetting<TQueryParameter> SetQueryWhenAllNotNull(
            Func<TQueryParameter, object?>[] propertySelectors,
            Func<TQueryParameter, Elastic.Clients.Elasticsearch.QueryDsl.Query?> query)
        {
            _settingList.Add(parameter =>
            {
                return  propertySelectors.All(selector => selector(parameter) == null) ? null : query(parameter);
            });
            return this;
        }

        internal IEnumerable<Elastic.Clients.Elasticsearch.QueryDsl.Query> GetQuery(TQueryParameter parameter)
        {
            return _settingList
                .Select(x => x(parameter))
                .Where(x => x != null)!;
        }

        internal SortSetting<TSortParameter> SetSort<TSortParameter>() where TSortParameter : Enum
        {
            var setting = new SortSetting<TSortParameter>();
            SortDict[typeof(TSortParameter)] = setting;
            return setting;
        }

        protected internal class SortSetting<TSortParameter> where TSortParameter : Enum
        {
            private readonly Dictionary<TSortParameter, Func<TQueryParameter, IList<SortOptions>>> _settingDict = new();

            internal SortSetting<TSortParameter> Set(TSortParameter sort,
                Func<TQueryParameter, IList<SortOptions>> sortCondition)
            {
                _settingDict[sort] = sortCondition;
                return this;
            }

            public Func<TQueryParameter, IList<SortOptions>>? GetSort(TSortParameter sort)
            {
                return _settingDict.GetValueOrDefault(sort);
            }
        }
    }
}