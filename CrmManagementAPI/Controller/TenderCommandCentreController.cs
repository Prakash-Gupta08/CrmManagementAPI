using CrmManagementAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderCommandCentreController : ControllerBase
    {
        private readonly ITenderCommandCentreService _service;

        public TenderCommandCentreController(ITenderCommandCentreService service)
        {
            _service = service;
        }

        // Whole page in one call
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll() => Ok(await _service.GetAll());

        // KPI cards: Active tenders, Weighted pipeline, Won, Win rate, Bids due 7 days, Pending Go/No-Go
        [HttpGet("GetSummary")]
        public async Task<ActionResult> GetSummary() => Ok(await _service.GetSummary());

        // "Pipeline value by stage" bar chart
        [HttpGet("GetPipelineByStage")]
        public async Task<ActionResult> GetPipelineByStage() => Ok(await _service.GetPipelineByStage());

        // "Active pipeline by customer category" donut
        [HttpGet("GetPipelineByCategory")]
        public async Task<ActionResult> GetPipelineByCategory() => Ok(await _service.GetPipelineByCategory());

        // "Upcoming bid deadlines"
        [HttpGet("GetUpcomingDeadlines")]
        public async Task<ActionResult> GetUpcomingDeadlines(int top = 15)
            => Ok(await _service.GetUpcomingDeadlines(top));

        // "Go/No-Go approval queue"
        [HttpGet("GetGoNoGoQueue")]
        public async Task<ActionResult> GetGoNoGoQueue(int top = 10)
            => Ok(await _service.GetGoNoGoQueue(top));

        // "Recent bid team activities"
        [HttpGet("GetRecentActivities")]
        public async Task<ActionResult> GetRecentActivities(int top = 10)
            => Ok(await _service.GetRecentActivities(top));

        // "Execution - Billing - Collection" cards
        [HttpGet("GetBillingSummary")]
        public async Task<ActionResult> GetBillingSummary() => Ok(await _service.GetBillingSummary());

        // "Purchase Order register"
        [HttpGet("GetPoRegister")]
        public async Task<ActionResult> GetPoRegister(int top = 20)
            => Ok(await _service.GetPoRegister(top));

        // "Accounts receivable aging"
        [HttpGet("GetReceivableAging")]
        public async Task<ActionResult> GetReceivableAging() => Ok(await _service.GetReceivableAging());

        // "Outstanding invoices"
        [HttpGet("GetOutstandingInvoices")]
        public async Task<ActionResult> GetOutstandingInvoices(int top = 15)
            => Ok(await _service.GetOutstandingInvoices(top));

        // "Upcoming project milestones"
        [HttpGet("GetUpcomingMilestones")]
        public async Task<ActionResult> GetUpcomingMilestones(int top = 10)
            => Ok(await _service.GetUpcomingMilestones(top));

        // "Tenders by portal" donut
        [HttpGet("GetTendersByPortal")]
        public async Task<ActionResult> GetTendersByPortal() => Ok(await _service.GetTendersByPortal());
    }
}
