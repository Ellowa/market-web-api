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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<Customer> _customers;

        public CustomerRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _customers = _tradeMarketDbContext.Set<Customer>();
        }

        public async Task AddAsync(Customer entity)
        {
            await _customers.AddAsync(entity);
        }

        public void Delete(Customer entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
             var entity = await _customers.FindAsync(id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customers.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await _customers
                .Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            return await _customers
                .Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(Customer entity)
        {
            _customers.Update(entity);
        }
    }
}
