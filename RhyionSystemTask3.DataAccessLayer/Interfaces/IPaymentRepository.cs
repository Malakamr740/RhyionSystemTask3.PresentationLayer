
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
    }
}
