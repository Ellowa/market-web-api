using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Business.Validation;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(CustomerModel model)
        {
            CustomerModelVaild(model);
            await _unitOfWork.CustomerRepository.AddAsync(_mapper.Map<Customer>(model));
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            if (modelId < 0) throw new MarketException("modelId must be greater than 0");

            await _unitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            if (id < 0) throw new MarketException("id must be greater than 0");

            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            if (productId < 0) throw new MarketException("productId must be greater than 0");

            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            var customersByProductId = customers.Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)));
            return _mapper.Map<IEnumerable<CustomerModel>>(customersByProductId);
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            CustomerModelVaild(model);

            var customer = _mapper.Map<Customer>(model);
            var person = _mapper.Map<Person>(model);
            customer.Person = person;
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveAsync();
        }

        private void CustomerModelVaild(CustomerModel model)
        {
            if (model == null) throw new MarketException("model is null");
            if (model.Id < 0) throw new MarketException("model.Id must be greater than 0");
            if (string.IsNullOrEmpty(model.Name)) throw new MarketException("model.Name is null or empty");
            if (string.IsNullOrEmpty(model.Surname)) throw new MarketException("model.Surname is null or empty");
            if (model.BirthDate > DateTime.Now) throw new MarketException("model.BirthDate is more than DateTime.Now");
            if (model.BirthDate < DateTime.Now.AddYears(-150)) throw new MarketException("model.BirthDate is less  than DateTime.Now - 150 Years");
            if (model.DiscountValue < 0) throw new MarketException("model.DiscountValue must be greater than 0");
        }
    }
}
