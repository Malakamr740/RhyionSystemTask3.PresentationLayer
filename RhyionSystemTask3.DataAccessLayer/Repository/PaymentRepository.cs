using Microsoft.EntityFrameworkCore;
using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDBContext context) : base(context) { }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _dbSet.SingleOrDefaultAsync(p => p.OrderId == orderId);
        }
    }
}
