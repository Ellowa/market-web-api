using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<Product> _products;

        public ProductRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _products = _tradeMarketDbContext.Set<Product>();
        }

        public async Task AddAsync(Product entity)
        {
            await _products.AddAsync(entity);
        }

        public void Delete(Product entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _products.FindAsync(id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _products
                .Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await _products
                .Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(Product entity)
        {
            _products.Update(entity);
        }
    }
}
