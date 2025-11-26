using Microsoft.EntityFrameworkCore;
using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDBContext context) : base(context) { }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
