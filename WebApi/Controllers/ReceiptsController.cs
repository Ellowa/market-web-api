using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        // GET: api/receipts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> Get()
        {
            return Ok(await _receiptService.GetAllAsync());
        }

        //GET: api/receipts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }
            return Ok(receipt);
        }

        // GET: api/receipts/1/details
        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetReceiptDetailsById(int id)
        {
            return Ok(await _receiptService.GetReceiptDetailsAsync(id));
        }

        // GET: api/receipts/{id}/sum
        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetSumById(int id)
        {
            return Ok(await _receiptService.ToPayAsync(id));
        }

        // GET: api/receipts/period?startDate=2021-12-1&endDate=2020-12-31
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetByPeriod(DateTime startDate, DateTime endDate)
        {
            return Ok(await _receiptService.GetReceiptsByPeriodAsync(startDate, endDate));
        }

        // POST: api/receipts
        [MarketValedationExceptionFilterAttribute]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ReceiptModel value)
        {
            await _receiptService.AddAsync(value);
            return Created("/receipts/" + value.Id, value);
        }

        // PUT: api/receipts/1
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReceiptModel value)
        {
            if (id != value.Id) return BadRequest();

            try
            {
                await _receiptService.UpdateAsync(value);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/receipts/1/products/add/4/2
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProduct(int id, int productId, int quantity)
        {
            try
            {
                await _receiptService.AddProductAsync(productId, id, quantity);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/receipts/1/products/remove/4/2
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProduct(int id, int productId, int quantity)
        {
            try
            {
                await _receiptService.RemoveProductAsync(productId, id, quantity);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT: api/receipts/1/checkout 
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> CheckOut(int id)
        {
            try
            {
                await _receiptService.CheckOutAsync(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/receipts/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _receiptService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _receiptService.DeleteAsync(id);
            return NoContent();
        }
    }
}
