
using RhyionSystemsTask3.BusinessLogicLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;
using RhyionSystemTask3.DataAccessLayer.UnitOfWork;

namespace RhyionSystemsTask3.BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product> GetProductDetailsAsync(int productId)
        {
            return await _unitOfWork.Products.GetByIdAsync(productId);
        }

        public async Task<bool> CheckProductAvailabilityAsync(int productId, int quantity)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);

            if (product == null) return false;

            return true;
        }
    }
}
