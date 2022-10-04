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
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<ProductCategory> _productCategories;

        public ProductCategoryRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _productCategories = _tradeMarketDbContext.Set<ProductCategory>();
        }

        public async Task AddAsync(ProductCategory entity)
        {
            await _productCategories.AddAsync(entity);
        }

        public void Delete(ProductCategory entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _productCategories.FindAsync(id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _productCategories.AsNoTracking().ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _productCategories.AsNoTracking().FirstOrDefaultAsync(pc => pc.Id == id);
        }

        public void Update(ProductCategory entity)
        {
            _productCategories.Update(entity);
        }
    }
}
