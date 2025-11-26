using Microsoft.EntityFrameworkCore;
using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.Interfaces;

namespace RhyionSystemTask3.DataAccessLayer.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly AppDBContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(AppDBContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);
        public async Task<TEntity> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();
        public void Update(TEntity entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void Delete(TEntity entity) => _dbSet.Remove(entity);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
