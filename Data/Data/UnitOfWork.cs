using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Data.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private IPersonRepository _personRepository;
        private ICustomerRepository _customerRepository;
        private IReceiptRepository _receiptRepository;
        private IReceiptDetailRepository _receiptDetailRepository;
        private IProductRepository _productRepository;
        private IProductCategoryRepository _productCategoryRepository;

        public UnitOfWork(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
        }

        public ICustomerRepository CustomerRepository => _customerRepository = _customerRepository ?? new CustomerRepository(_tradeMarketDbContext);

        public IPersonRepository PersonRepository => _personRepository = _personRepository ?? new PersonRepository(_tradeMarketDbContext);

        public IProductRepository ProductRepository => _productRepository = _productRepository ?? new ProductRepository(_tradeMarketDbContext);

        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository = _productCategoryRepository ?? new ProductCategoryRepository(_tradeMarketDbContext);

        public IReceiptRepository ReceiptRepository => _receiptRepository = _receiptRepository ?? new ReceiptRepository(_tradeMarketDbContext);

        public IReceiptDetailRepository ReceiptDetailRepository => _receiptDetailRepository = _receiptDetailRepository ?? new ReceiptDetailRepository(_tradeMarketDbContext);

        public async Task SaveAsync()
        {
            await _tradeMarketDbContext.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _tradeMarketDbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
