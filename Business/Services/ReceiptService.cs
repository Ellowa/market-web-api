using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Business.Interfaces;
using System.Threading.Tasks;
using Business.Models;
using Data.Interfaces;
using AutoMapper;
using Data.Entities;
using Business.Validation;

namespace Business.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            ReceiptModelVaild(model);

            await _unitOfWork.ReceiptRepository.AddAsync(_mapper.Map<Receipt>(model));
            await _unitOfWork.SaveAsync();
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            if (productId < 0) throw new MarketException("productId must be greater than 0");
            if (receiptId < 0) throw new MarketException("receiptId must be greater than 0");
            if (quantity < 0) throw new MarketException("quantity must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt == null) throw new MarketException("receipt by receiptId is null");

            var receiptDetail = receipt.ReceiptDetails?.FirstOrDefault(rd => rd.ProductId == productId);
            if (receiptDetail == null)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null) throw new MarketException("product by productId is null");

                receiptDetail = new ReceiptDetail()
                {
                    ReceiptId = receiptId,
                    ProductId = productId,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = product.Price - (product.Price * receipt.Customer.DiscountValue / 100),
                    Quantity = quantity,
                    Receipt = receipt,
                    Product = product
                };
                await _unitOfWork.ReceiptDetailRepository.AddAsync(receiptDetail);
            }
            else
            {
                receiptDetail.Quantity += quantity;
                _unitOfWork.ReceiptDetailRepository.Update(receiptDetail);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task CheckOutAsync(int receiptId)
        {
            if (receiptId < 0) throw new MarketException("receiptId must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdAsync(receiptId);
            receipt.IsCheckedOut = true;
            _unitOfWork.ReceiptRepository.Update(receipt);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            if (modelId < 0) throw new MarketException("modelId must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

            foreach (var receiptsDetails in receipt.ReceiptDetails)
                _unitOfWork.ReceiptDetailRepository.Delete(receiptsDetails);

            await _unitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReceiptModel>>(receipts);
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            if (id < 0) throw new MarketException("id must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<ReceiptModel>(receipt);
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            if (receiptId < 0) throw new MarketException("receiptId must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            return _mapper.Map<IEnumerable<ReceiptDetailModel>>(receipt.ReceiptDetails);
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate) throw new MarketException();

            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReceiptModel>>(receipts.Where(r => r.OperationDate > startDate && r.OperationDate < endDate));
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            if (productId < 0) throw new MarketException("productId must be greater than 0");
            if (receiptId < 0) throw new MarketException("receiptId must be greater than 0");
            if (quantity < 0) throw new MarketException("quantity must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            var receiptDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);
            receiptDetail.Quantity -= quantity;
            
            if (receiptDetail.Quantity > 0)
            {
                receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId).Quantity = receiptDetail.Quantity;
                _unitOfWork.ReceiptRepository.Update(receipt);
                //_unitOfWork.ReceiptDetailRepository.Update(receiptDetail);
            }
            else
                _unitOfWork.ReceiptDetailRepository.Delete(receiptDetail);
            await _unitOfWork.SaveAsync();
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            if (receiptId < 0) throw new MarketException("receiptId must be greater than 0");

            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);
            return receipt.ReceiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity);
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            ReceiptModelVaild(model);

            var receipt = _mapper.Map<Receipt>(model);
            _unitOfWork.ReceiptRepository.Update(receipt);
            await _unitOfWork.SaveAsync();
        }

        private void ReceiptModelVaild(ReceiptModel model)
        {
            if (model == null) throw new MarketException("model is null");
            if (model.Id < 0) throw new MarketException("model.Id must be greater than 0");
            if (model.CustomerId < 0) throw new MarketException("model.CustomerId must be greater than 0");
            if (model.OperationDate > DateTime.Now) throw new MarketException("model.OperationDate is more than DateTime.Now");
            if (model.OperationDate < DateTime.Now.AddYears(-150)) throw new MarketException("model.OperationDate is less  than DateTime.Now - 150 Years");
            if (model.ReceiptDetailsIds == null) throw new MarketException("model.ReceiptDetailsIds is null");
        }
    }
}
