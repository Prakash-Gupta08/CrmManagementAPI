using CrmManagementAPI.CommonResponse;
using CrmManagementAPI.Interfaces;
using CrmManagementAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrmManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _context;
        protected APIResponse _response;
        public CustomerController(ICustomerService context)
        {
            _context = context;
            _response = new();
        }
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllCustomer(string? search = null, int pageNumber = 1, int pageSize = 10)
        {
            var data = await _context.GetAllCustomer( search, pageNumber, pageSize);

            return Ok(data);


        }
        [HttpGet("GetCustomerByID")]
        public async Task<ActionResult> GetCustomerById(int id)
        {
            var data = await _context.GetCustomerById(id);
            return Ok(data); 
        }

        [HttpPost("CreateCustomer")]
        public async Task<ActionResult> CreateCustomer([FromBody] CreateCustomer dto)
        {
            var data = await _context.CreateCustomer(dto);
            if (data == null)
            {
                return null;
            }
            _response.IsSuccess = true;
            _response.ActionResponse = "data created";
            return Ok(data);
        }

        [HttpPost("UpdateCustomer")]
        public async Task<ActionResult> UpdateCustomer([FromBody] EditCustomer req)
        {
            var data = await _context.UpdateCustomer(req);
            if (data == null)
            {

                return null;
            }
            _response.IsSuccess = true;
            _response.ActionResponse = "data found";

            return Ok(data);
        }

        [HttpDelete("{id}")] 
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var data = await _context.DeleteCustomer(id);
            return Ok();

        }


    }
}
