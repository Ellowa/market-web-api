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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        // GET: api/products?categoryId=1&minPrice=20&maxPrice=50
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductModel>>> Get([FromQuery]FilterSearchModel filter)
        {
            if (filter != null)
            {
                return Ok(await _productService.GetByFilterAsync(filter));
            }
            return Ok(await _productService.GetAllAsync());
        }

        //GET: api/products/1
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/products
        [MarketValedationExceptionFilterAttribute]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Add([FromBody] ProductModel value)
        {
            await _productService.AddAsync(value);
            return Created("/products/" + value.Id, value);
        }

        // PUT: api/products/1
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(int id, [FromBody] ProductModel value)
        {
            if (id != value.Id) return BadRequest();

            try
            {
                await _productService.UpdateAsync(value);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(id);
            return NoContent();
        }


        // GET: api/products/categories
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetCategories()
        {
            return Ok(await _productService.GetAllProductCategoriesAsync());
        }

        // POST: api/products/categories
        [MarketValedationExceptionFilterAttribute]
        [HttpPost("categories")]
        public async Task<ActionResult> AddCategory([FromBody] ProductCategoryModel value)
        {
            await _productService.AddCategoryAsync(value);
            return Created("", value);
        }

        // PUT: api/products/categories/1
        [MarketValedationExceptionFilterAttribute]
        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] ProductCategoryModel value)
        {
            if (id != value.Id) return BadRequest();

            try
            {
                await _productService.UpdateCategoryAsync(value);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/products/categories/1
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            await _productService.RemoveCategoryAsync(id);
            return NoContent();
        }
    }
}
