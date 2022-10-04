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
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<Receipt> _receipts;

        public ReceiptRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _receipts = _tradeMarketDbContext.Set<Receipt>();
        }

        public async Task AddAsync(Receipt entity)
        {
            // When we use identity PK
            if (entity.Id != 0)
                entity.Id = 0;
            await _receipts.AddAsync(entity);
        }

        public void Delete(Receipt entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _receipts.FindAsync(id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await _receipts.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await _receipts
                .Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Receipt> GetByIdAsync(int id)
        {
            return await _receipts.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return await _receipts
                .Include(r => r.Customer)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Product)
                .ThenInclude(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public void Update(Receipt entity)
        {
            _receipts.Update(entity);
        }
    }
}
