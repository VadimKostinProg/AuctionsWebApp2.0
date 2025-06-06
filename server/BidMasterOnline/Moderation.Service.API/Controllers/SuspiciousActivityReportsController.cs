using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Moderation.Service.API.Enums;
using Moderation.Service.API.Models;
using Moderation.Service.API.ServiceContracts;
using System.ComponentModel.DataAnnotations;

namespace Moderation.Service.API.Controllers
{
    [Route("api/moderator/suspicious-activity-reports")]
    [ApiController]
    public class SuspiciousActivityReportsController : BaseController
    {
        private readonly ISuspiciousActivityReportsService _service;

        public SuspiciousActivityReportsController(ISuspiciousActivityReportsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuspiciousActivityReport(
            [FromQuery, Required] SuspiciousActivityReportPeriod period)
        {
            ServiceResult<SuspiciousActivityReport> result = await _service.GetSuspiciousActivityReportAsync(period);

            return FromResult(result);
        }

        [HttpGet("auction-analyses/{id}")]
        public async Task<IActionResult> GetAuctionAnalyses([FromRoute] string id)
        {
            ServiceResult<SuspiciousActivityReportAuctionAnalysis> result = 
                await _service.GetAuctionAnalysisAsync(id);

            return FromResult(result);
        }
    }
}
