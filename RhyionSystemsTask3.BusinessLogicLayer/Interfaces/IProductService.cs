
using RhyionSystemTask3.DataAccessLayer.Models;

namespace RhyionSystemsTask3.BusinessLogicLayer.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<Product> GetProductDetailsAsync(int productId);
        Task<bool> CheckProductAvailabilityAsync(int productId, int quantity);
    }
}
