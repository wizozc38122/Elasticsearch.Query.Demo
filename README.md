# Elasticsearch.Query.Demo

介面隔離實作，Repository不直接吐出DocumentModel, 而是DocumentModel轉換DataModel

Repository僅實踐Service所需要的外部資源介面

未來跟換資料庫等，仍可不同實作 (雖然情境上很難:D)

設計QuerySetting, 起初是為了在實作Repository時會重複使用`Field<T>`

不停重複指定DocumentModel, 但其實就是針對同一個Index查詢

設計參考Mapper相關可以直接Set的方式去定義

並且針對不同查詢參數，可以吐出不同查詢條件，針對同一個DocumentModel

```yaml
依賴套件:
  Elasticsearch 7.x: NEST
  Elasticsearch 8.x: Elastic.Clients.Elasticsearch
```

# TestData
`[PUT] /shop`
```json
{
  "mappings": {
    "properties": {
      "shopId": {
        "type": "integer"
      },
      "name": {
        "type": "text"
      },
      "revenue": {
        "type": "double"
      },
      "location": {
        "type": "geo_point"
      }
    }
  }
}
```

`[POST] /_bulk`
```json
{ "index": { "_index": "shop", "_id": "1" } }
{ "shopId": 1, "name": "Shop A", "revenue": 123456.78, "location": { "lat": 25.034, "lon": 121.564 } }
{ "index": { "_index": "shop", "_id": "2" } }
{ "shopId": 2, "name": "Shop B", "revenue": 234567.89, "location": { "lat": 24.803, "lon": 121.577 } }
{ "index": { "_index": "shop", "_id": "3" } }
{ "shopId": 3, "name": "Shop C", "revenue": 345678.90, "location": { "lat": 25.032, "lon": 121.565 } }
{ "index": { "_index": "shop", "_id": "4" } }
{ "shopId": 4, "name": "Shop D", "revenue": 456789.01, "location": { "lat": 25.033, "lon": 121.563 } }
{ "index": { "_index": "shop", "_id": "5" } }
{ "shopId": 5, "name": "Shop E", "revenue": 567890.12, "location": { "lat": 25.031, "lon": 121.566 } }
{ "index": { "_index": "shop", "_id": "6" } }
{ "shopId": 6, "name": "Shop F", "revenue": 678901.23, "location": { "lat": 24.800, "lon": 121.579 } }
{ "index": { "_index": "shop", "_id": "7" } }
{ "shopId": 7, "name": "Shop G", "revenue": 789012.34, "location": { "lat": 25.030, "lon": 121.568 } }
{ "index": { "_index": "shop", "_id": "8" } }
{ "shopId": 8, "name": "Shop H", "revenue": 890123.45, "location": { "lat": 25.029, "lon": 121.570 } }
{ "index": { "_index": "shop", "_id": "9" } }
{ "shopId": 9, "name": "Shop I", "revenue": 901234.56, "location": { "lat": 25.028, "lon": 121.571 } }
{ "index": { "_index": "shop", "_id": "10" } }
{ "shopId": 10, "name": "Shop J", "revenue": 123456.78, "location": { "lat": 25.035, "lon": 121.572 } }
```