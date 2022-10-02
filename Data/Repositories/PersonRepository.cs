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
    public class PersonRepository : IPersonRepository
    {
        private readonly TradeMarketDbContext _tradeMarketDbContext;
        private readonly DbSet<Person> _persons;

        public PersonRepository(TradeMarketDbContext tradeMarketDbContext)
        {
            _tradeMarketDbContext = tradeMarketDbContext;
            _persons = _tradeMarketDbContext.Set<Person>();
        }

        public async Task AddAsync(Person entity)
        {
            await _persons.AddAsync(entity);
        }

        public void Delete(Person entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await _persons.FindAsync(id);
            _tradeMarketDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _persons.ToListAsync();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            return await _persons.FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Update(Person entity)
        {
            _tradeMarketDbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
