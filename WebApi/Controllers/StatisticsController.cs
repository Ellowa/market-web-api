using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        // GET: api/statistic/popularProducts?productCount=2
        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetPopularProducts(int productCount)
        {
            return Ok(await _statisticService.GetMostPopularProductsAsync(productCount));
        }

        //GET: api/statisic/customer/1/2
        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetPopularProductsOfCustomer(int id, int productCount)
        {
            return Ok(await _statisticService.GetCustomersMostPopularProductsAsync(productCount, id));
        }

        // GET: api/statistic/activity/{customerCount}?startDate= 2020-7-21&endDate= 2020-7-2
        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostActiveCustomers(int customerCount, DateTime startDate, DateTime endDate)
        {
            return Ok(await _statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate));
        }

        // GET: api/statistic/income/{categoryId}?startDate= 2020-7-21&endDate= 2020-7-22
        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeOfCategory(int categoryId, DateTime startDate, DateTime endDate)
        {
            return Ok(await _statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate));
        }
    }
}
