using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            if (productCount < 0) throw new MarketException("productCount must be greater than 0");
            if (customerId < 0) throw new MarketException("customerId must be greater than 0");

            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var mostPopularProducts = receipts
                .Where(r => r.CustomerId == customerId)
                .SelectMany(r => r.ReceiptDetails)
                .GroupBy(rd => rd.ProductId)
                .OrderByDescending(rd => rd.Sum(x => x.Quantity))
                .Take(productCount).Select(x => x.Select(t => t.Product).FirstOrDefault());
            return _mapper.Map<IEnumerable<ProductModel>>(mostPopularProducts);
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            if (categoryId < 0) throw new MarketException("categoryId must be greater than 0");
            if (endDate < startDate) throw new MarketException();

            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var incomeOfCategoryInPeriod = receipts
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .Sum(rd => rd.DiscountUnitPrice * rd.Quantity);
            return incomeOfCategoryInPeriod;
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            if (productCount < 0) throw new MarketException("productCount must be greater than 0");

            var receiptdetails = await _unitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync();

            var mostPopularProducts = receiptdetails
                .GroupBy(rd => rd.ProductId)
                .OrderByDescending(rd => rd.Sum(x => x.Quantity))
                .Take(productCount).Select(x => x.Select(t => t.Product).FirstOrDefault()).ToList();

            foreach(var mostPopularProduct in mostPopularProducts)
            {
                mostPopularProduct.ReceiptDetails = receiptdetails.Where(rd => rd.ProductId == mostPopularProduct.Id).ToList();
            }

            return _mapper.Map<IEnumerable<ProductModel>>(mostPopularProducts);
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            if (customerCount < 0) throw new MarketException("categoryId must be greater than 0");
            if (endDate < startDate) throw new MarketException();

            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            var topCustomerActivityModels = receipts

                .GroupBy(r => r.CustomerId, (key, x) =>
                    new CustomerActivityModel
                    {
                        CustomerId = key,
                        CustomerName = x.Select(t => t.Customer.Person.Name + " " + t.Customer.Person.Surname).FirstOrDefault(),
                        ReceiptSum = x.SelectMany(t => t.ReceiptDetails).Sum(rd => rd.DiscountUnitPrice * rd.Quantity)
                    })
                .OrderByDescending(cam => cam.ReceiptSum)
                .Take(customerCount);
            return topCustomerActivityModels;
        }
    }
}
