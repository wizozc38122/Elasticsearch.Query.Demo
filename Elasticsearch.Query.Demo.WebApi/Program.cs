using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Elasticsearch.Net;
using ES7 = Elasticsearch.Query.Demo.Repository.Elasticsearch7;
using ES8 = Elasticsearch.Query.Demo.Repository.Elasticsearch8;
using Elasticsearch.Query.Demo.Service.ShopList;
using Elasticsearch.Query.Demo.Service.ShopList.Port.In;
using Elasticsearch.Query.Demo.Service.ShopList.Port.Out;
using Mapster;
using Nest;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Mapster
builder.Services.AddMapster();

// Repository
var esVersion = builder.Configuration.GetSection("ElasticsearchVersion").Get<int>();
var esNodes = builder.Configuration.GetSection("Elasticsearch").Get<IEnumerable<string>>() ?? throw new Exception("Elasticsearch nodes not found");
if (esVersion == 7)
{
    builder.Services.AddSingleton<IElasticClient>(sp =>
    {
        var connectionPool = new StaticConnectionPool(esNodes.Select(x => new Uri(x)));
        var settings = new ConnectionSettings(connectionPool);
        var client = new ElasticClient(settings);
        return client;
    });
    
    builder.Services.AddSingleton<ES7.DemoElasticsearch>();
    builder.Services.AddScoped<IShopListPort, ES7.Implementation.ShopListRepository>();
}
else
{
    builder.Services.AddSingleton<ElasticsearchClient>(sp =>
    {
        var pool = new StaticNodePool(esNodes.Select(x => new Uri(x)));
        var settings = new ElasticsearchClientSettings(pool);
        settings.DisableDirectStreaming();
        var client = new ElasticsearchClient(settings);
        return client;
    });
    builder.Services.AddSingleton<ES8.DemoElasticsearch>();
    builder.Services.AddScoped<IShopListPort, ES8.Implementation.ShopListRepository>();
}



// Service
builder.Services.AddScoped<IShopListService, ShopListService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();