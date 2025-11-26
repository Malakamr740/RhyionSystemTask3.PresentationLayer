
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemTask3.DataAccessLayer.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order> GetOrderDetailsAsync(int orderId);
    }
}
