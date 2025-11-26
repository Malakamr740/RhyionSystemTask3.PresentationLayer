
using Microsoft.EntityFrameworkCore;
using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Repository
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDBContext context) : base(context) { }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
           return await _dbSet
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product) 
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
