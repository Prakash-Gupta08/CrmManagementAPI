using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _context;
        protected APIResponse _response;
        public PaymentController(IPaymentService context)
        {
            _context = context;
            _response = new();
        }

        [HttpGet("GetAllPayment")]
        public async Task<ActionResult> GetAllPayment(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllPayment(search, pageNumber, pageSize);
            return Ok(data);
        }

        [HttpGet("GetPaymentByID")]
        public async Task<ActionResult> GetPaymentById(int id)
        {
            var data = await _context.GetPaymentById(id);
            return Ok(data);
        }

        [HttpPost("CreatePayment")]
        public async Task<ActionResult> CreatePayment([FromBody] CreatePayment dto)
        {
            var data = await _context.CreatePayment(dto);
            return Ok(data);
        }

        [HttpPost("UpdatePayment")]
        public async Task<ActionResult> UpdatePayment([FromBody] EditPayment req)
        {
            var data = await _context.UpdatePayment(req);
            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            var data = await _context.DeletePayment(id);
            return Ok(data);
        }
    }
}
