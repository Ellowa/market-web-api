using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            return Ok(await _customerService.GetAllAsync());
        }

        //GET: api/customers/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
            return Ok(await _customerService.GetByIdAsync(id));
        }
        
        //GET: api/customers/products/1
        [HttpGet("products/{id}")]
        public async Task<ActionResult<CustomerModel>> GetByProductId(int id)
        {
            return Ok(await _customerService.GetCustomersByProductIdAsync(id));
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CustomerModel value)
        {
            await _customerService.AddAsync(value);
            return Created("/customer/" + value.Id, value);
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int Id, [FromBody] CustomerModel value)
        {
            if (Id != value.Id)
            {
                return BadRequest();
            }
            if(_customerService.GetCustomersByProductIdAsync(Id) == null)
            {
                return NotFound();
            }    

            await _customerService.UpdateAsync(value);
            return NoContent();
        }

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (_customerService.GetCustomersByProductIdAsync(id) == null)
            {
                return NotFound();
            }

            await _customerService.DeleteAsync(id);
            return NoContent();
        }
    }
}
