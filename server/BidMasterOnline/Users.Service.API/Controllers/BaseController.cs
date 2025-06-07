using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Users.Service.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult FromResult<T>(ServiceResult<T> result)
            => result.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => Ok(result),
                System.Net.HttpStatusCode.NotFound => NotFound(result),
                System.Net.HttpStatusCode.BadRequest => BadRequest(result),
                _ => BadRequest(result)
            };

        protected IActionResult FromResult(ServiceResult result)
            => result.StatusCode switch
            {
                System.Net.HttpStatusCode.OK => Ok(result),
                System.Net.HttpStatusCode.NotFound => NotFound(result),
                System.Net.HttpStatusCode.BadRequest => BadRequest(result),
                _ => BadRequest(result)
            };
    }
}
