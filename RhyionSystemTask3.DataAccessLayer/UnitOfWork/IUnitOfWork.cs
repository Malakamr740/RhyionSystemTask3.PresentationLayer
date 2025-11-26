
using RhyionSystemTask3.DataAccessLayer.Interfaces;

namespace RhyionSystemTask3.DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IPaymentRepository Payments { get; }

        Task<int> CommitAsync();
    }
}
