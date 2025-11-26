using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Repository;

namespace RhyionSystemTask3.DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDBContext _context;

        public IUserRepository Users { get; }
        public IProductRepository Products { get; }
        public IOrderRepository Orders { get; }
        public IPaymentRepository Payments { get; }

        public UnitOfWork(AppDBContext context)
        {
            _context = context;

            Users = new UserRepository(context);
            Products = new ProductRepository(context);
            Orders = new OrderRepository(context);
            Payments = new PaymentRepository(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
