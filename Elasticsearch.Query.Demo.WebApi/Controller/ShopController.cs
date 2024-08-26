using Elasticsearch.Query.Demo.Service.ShopList.Port.In;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.Query.Demo.WebApi.Controller;

[ApiController]
[Route("[controller]")]
public class ShopController(IShopListService shopListService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetListAsync([FromQuery]ShopListParameter parameter)
    {
        var result = await shopListService.GetAsync(parameter);
        return Ok(result);
    }
}