using Microsoft.EntityFrameworkCore;
using MinimalChatAppApi.Data;
using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;
using System.Linq.Expressions;

namespace MinimalChatAppApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ChatContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ChatContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.Where(filter).ToListAsync();
            }
            else
            {
                return await _dbSet.ToListAsync();
            }
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }

}
