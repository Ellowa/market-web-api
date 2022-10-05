using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ReceiptDetailRepository : IReceiptDetailRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<ReceiptDetail> _receiptDetails;

        public ReceiptDetailRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _receiptDetails = _tradeMarketDbContext.Set<ReceiptDetail>();
        }

        public async Task AddAsync(ReceiptDetail entity)
        {
            // When we use identity PK
            if (entity.Id != 0)
                entity.Id = 0;
            await _receiptDetails.AddAsync(entity);
        }

        public void Delete(ReceiptDetail entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _receiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllAsync()
        {
            return await _receiptDetails.ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
        {
            return await _receiptDetails
                .Include(rd => rd.Receipt)
                .Include(rd => rd.Product)
                .ThenInclude(p => p.Category)
                
                .ToListAsync();
        }

        public async Task<ReceiptDetail> GetByIdAsync(int id)
        {
            return await _receiptDetails.FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task<ReceiptDetail> GetByIdWithDetailsAsync(int id)
        {
            return await _receiptDetails
                .Include(rd => rd.Receipt)
                .Include(rd => rd.Product)
                .ThenInclude(p => p.Category)
                
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(ReceiptDetail entity)
        {
            _receiptDetails.Update(entity);
        }
    }
}
