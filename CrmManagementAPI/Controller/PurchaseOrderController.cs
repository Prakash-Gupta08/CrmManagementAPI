using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _context;
        protected APIResponse _response;
        public PurchaseOrderController(IPurchaseOrderService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllPurchaseOrder")]
        public async Task<ActionResult> GetAllPurchaseOrder(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllPurchaseOrder(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetPurchaseOrderByID")]
        public async Task<ActionResult> GetPurchaseOrderById(int id)
        {
            var data = await _context.GetPurchaseOrderById(id);
            return Ok(data);
        }

        [HttpPost("CreatePurchaseOrder")]
        public async Task<ActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrder dto)
        {
            var data = await _context.CreatePurchaseOrder(dto);
            return Ok(data);
        }

        [HttpPost("UpdatePurchaseOrder")]
        public async Task<ActionResult> UpdatePurchaseOrder([FromBody] EditPurchaseOrder req)
        {
            var data = await _context.UpdatePurchaseOrder(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePurchaseOrder(int id)
        {
            var data = await _context.DeletePurchaseOrder(id);
            return Ok(data);
        }
    }
}
