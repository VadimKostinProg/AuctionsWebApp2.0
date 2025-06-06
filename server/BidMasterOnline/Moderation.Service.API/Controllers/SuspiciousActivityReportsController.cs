using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Moderation.Service.API.Enums;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Controllers
{
    [Route("api/moderator/suspicious-activity-reports")]
    [ApiController]
    public class SuspiciousActivityReportsController : BaseController
    {
        private readonly ISuspiciousActivityCheckService _service;

        public SuspiciousActivityReportsController(ISuspiciousActivityCheckService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateSuspiciousActivityReport([FromQuery] SuspiciousActivityReportPeriod period)
        {
            ServiceResult<object> result = await _service.GenerateSuspiciousActivityReportAsync(period);

            return FromResult(result);
        }
    }
}
