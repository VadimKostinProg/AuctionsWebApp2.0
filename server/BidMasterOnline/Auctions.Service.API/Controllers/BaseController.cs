using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult FromResult<T>(Result<T> result)
            => result.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => Ok(result),
                System.Net.HttpStatusCode.NotFound => NotFound(result),
                _ => BadRequest(result)
            };
    }
}
