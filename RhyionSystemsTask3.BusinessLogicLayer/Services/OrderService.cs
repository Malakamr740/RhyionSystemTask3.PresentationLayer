
using RhyionSystemsTask3.BusinessLogicLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Models;
using RhyionSystemTask3.DataAccessLayer.UnitOfWork;


namespace RhyionSystemTask3.PresentationLayer
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;



        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IPaymentRepository paymentRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> PlaceNewOrderAsync(int userId, Dictionary<int, int> productQuantities)
        {
            if (!productQuantities.Any()) throw new ArgumentException("Order must contain products.");
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new InvalidOperationException($"User {userId} not found.");

            decimal subtotal = 0;
            var orderProducts = new List<OrderProduct>();

            foreach (var item in productQuantities)
            {
                var product = await _productRepository.GetByIdAsync(item.Key) ?? throw new InvalidOperationException($"Product {item.Key} not found.");
                if (item.Value <= 0) throw new ArgumentException("Quantity must be positive.");

                subtotal += product.Price * item.Value;
                orderProducts.Add(new OrderProduct { ProductId = item.Key, Quantity = item.Value });
            }

              
            decimal tax = subtotal * 0.05m;
            decimal shippingFee = subtotal >= 50.00m ? 0m : 5.00m;
            decimal total = subtotal + tax + shippingFee;

                
            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = total,
                OrderProducts = orderProducts,
            };

            await _unitOfWork.Orders.AddAsync(newOrder);
            await _unitOfWork.CommitAsync();

            return newOrder.OrderId;
        }

        public async Task<bool> ProcessPaymentForOrderAsync(int orderId, string paymentMethod, decimal paymentAmount)
        {
            var order = await _unitOfWork.Orders.GetOrderDetailsAsync(orderId);

            if (order == null) return false;
            if (order.Payment != null) throw new InvalidOperationException($"Order {orderId} is already paid.");
            if (paymentAmount < order.TotalAmount) throw new ArgumentException("Payment amount is less than the order total.");

            bool paymentSuccessful = SimulatePaymentGateway(paymentAmount, paymentMethod);

            if (paymentSuccessful)
            {
                var payment = new Payment
                {
                    OrderId = orderId,
                    PaymentMethod = paymentMethod,
                    Amount = order.TotalAmount,
                    PaymentDate = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);
                await _unitOfWork.CommitAsync();

                return true;
            }

            return false;
        }

        public async Task<Order> GetOrderHistoryDetailsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderDetailsAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order ID {orderId} not found.");
            }

            return order;
        }

        private bool SimulatePaymentGateway(decimal amount, string method)
        {
                
            return amount > 0 && !method.Contains("Fail");
        }
    }
}

