
namespace RhyionSystemsTask3.BusinessLogicLayer.Interfaces
{
    public interface IOrderService
    {
        Task<int> PlaceNewOrderAsync(int userId, Dictionary<int, int> productQuantities);
        Task<bool> ProcessPaymentForOrderAsync(int orderId, string paymentMethod, decimal paymentAmount);
    }
}
