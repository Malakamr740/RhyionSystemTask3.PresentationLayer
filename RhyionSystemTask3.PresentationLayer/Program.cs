using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RhyionSystemTask3.DataAccessLayer.Models;
using RhyionSystemsTask3.BusinessLogicLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.UnitOfWork;
using RhyionSystemsTask3.BusinessLogicLayer.Services;
using RhyionSystemTask3.DataAccessLayer.Repository;
using RhyionSystemTask3.DataAccessLayer.Context;

namespace RhyionSystemTask3.PresentationLayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => ConfigureAppServices(services))
                .Build();

            using var scope = host.Services.CreateScope();
            var sp = scope.ServiceProvider;

            // Presentation only resolves BLL interfaces
            var userService = sp.GetService<IUserService>() ?? throw new InvalidOperationException("IUserService not registered.");
            var productService = sp.GetService<IProductService>() ?? throw new InvalidOperationException("IProductService not registered.");
            var orderService = sp.GetService<IOrderService>() ?? throw new InvalidOperationException("IOrderService not registered.");
            var reportService = sp.GetService<IReportService>(); // optional

            // If BLL supports a seed path, it should expose it; otherwise inform the user.
            await EnsureProductsPresentAsync(productService);

            Console.WriteLine("=== E-COMMERCE INTERACTIVE CONSOLE (BLL-only) ===");
            var exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("Select an option:");
                Console.WriteLine(" 1) Manage Users");
                Console.WriteLine(" 2) Manage Products");
                Console.WriteLine(" 3) Place Order");
                Console.WriteLine(" 4) Process Payment");
                Console.WriteLine(" 5) Monthly Sales Report");
                Console.WriteLine(" 0) Exit");
                Console.Write("Choice: ");
                var choice = Console.ReadLine()?.Trim();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ManageUsersAsync(userService);
                            break;
                        case "2":
                            await ManageProductsAsync(productService);
                            break;
                        case "3":
                            await PlaceOrderInteractiveAsync(orderService, productService, userService);
                            break;
                        case "4":
                            await ProcessPaymentInteractiveAsync(orderService);
                            break;
                        case "5":
                            await GenerateMonthlyReportAsync(reportService);
                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Exiting. Goodbye.");
        }

        private static async Task ManageUsersAsync(IUserService userService)
        {
            Console.WriteLine("\n--- Users ---");
            Console.WriteLine(" a) Create user");
            Console.WriteLine(" b) View user profile by ID");
            Console.WriteLine(" c) Update user email");
            Console.Write("Choice: ");
            var sub = Console.ReadLine()?.Trim()?.ToLower();

            switch (sub)
            {
                case "a":
                    Console.Write("First name: ");
                    var fn = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Last name: ");
                    var ln = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Email: ");
                    var email = Console.ReadLine()?.Trim() ?? "";

                    int createdId = await userService.RegisterNewUserAsync(fn, ln, email, "placeholder-hash");
                    Console.WriteLine($"User created (BLL) with ID: {createdId}");
                    break;
                case "b":
                    var id = ReadInt("User ID: ");
                    var profile = await userService.GetUserProfileAsync(id);
                    if (profile == null) Console.WriteLine("User not found.");
                    else Console.WriteLine($" {profile.UserId}: {profile.FirstName} {profile.LastName} <{profile.Email}>");
                    break;
                case "c":
                    var uid = ReadInt("User ID to update: ");
                    Console.Write("New email: ");
                    var newEmail = Console.ReadLine()?.Trim() ?? "";
                    var updated = await userService.UpdateUserProfileAsync(uid, newEmail);
                    Console.WriteLine(updated ? "User updated (BLL)." : "Update failed or user not found.");
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
        private static async Task ManageProductsAsync(IProductService productService)
        {
            Console.WriteLine("\n--- Products ---");
            Console.WriteLine(" a) List available products");
            Console.WriteLine(" b) View product details by ID");
            Console.WriteLine(" c) Check product availability (quantity)");
            Console.Write("Choice: ");
            var sub = Console.ReadLine()?.Trim()?.ToLower();

            switch (sub)
            {
                case "a":
                    var products = await productService.GetAvailableProductsAsync();
                    Console.WriteLine($"Total products: {products.Count()}");
                    foreach (var p in products)
                        Console.WriteLine($" {p.ProductId}: {p.Name} - {p.Price:C} - {p.Description}");
                    break;
                case "b":
                    var pid = ReadInt("Product ID: ");
                    var details = await productService.GetProductDetailsAsync(pid);
                    if (details == null) Console.WriteLine("Product not found.");
                    else Console.WriteLine($" {details.ProductId}: {details.Name} - {details.Price:C} - {details.Description}");
                    break;
                case "c":
                    var checkId = ReadInt("Product ID to check: ");
                    var qty = ReadInt("Requested quantity: ");
                    var available = await productService.CheckProductAvailabilityAsync(checkId, qty);
                    Console.WriteLine(available ? "Product is available in requested quantity." : "Insufficient stock or product not found.");
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
        private static async Task PlaceOrderInteractiveAsync(IOrderService orderService, IProductService productService, IUserService userService)
        {
            Console.WriteLine("\n--- Place Order ---");
            var userId = ReadInt("Enter User ID placing the order: ");
            var user = await userService.GetUserProfileAsync(userId);
            if (user == null) { Console.WriteLine("User not found."); return; }

            var items = new Dictionary<int, int>();
            while (true)
            {
                var pid = ReadInt("Product ID to add (0 to finish): ");
                if (pid == 0) break;
                var product = await productService.GetProductDetailsAsync(pid);
                if (product == null) { Console.WriteLine("Product not found."); continue; }
                var qty = ReadInt($"Quantity for '{product.Name}': ");
                if (qty <= 0) { Console.WriteLine("Quantity must be positive."); continue; }

                var isAvailable = await productService.CheckProductAvailabilityAsync(pid, qty);
                if (!isAvailable) { Console.WriteLine("Requested quantity not available."); continue; }

                if (items.ContainsKey(pid)) items[pid] += qty;
                else items[pid] = qty;
                Console.WriteLine("Item added.");
            }

            if (!items.Any()) { Console.WriteLine("No items selected."); return; }

            int orderId = await orderService.PlaceNewOrderAsync(userId, items);
            if (orderId <= 0) Console.WriteLine("Order placement failed (BLL).");
            else Console.WriteLine($"Order created (BLL) with ID: {orderId}");
        }

        private static async Task ProcessPaymentInteractiveAsync(IOrderService orderService)
        {
            Console.WriteLine("\n--- Process Payment ---");
            var orderId = ReadInt("Order ID to pay: ");
            var method = Prompt("Payment method (e.g. CreditCard): ");
            var amount = ReadDecimal("Amount to pay: ");
            bool success = await orderService.ProcessPaymentForOrderAsync(orderId, method, amount);
            Console.WriteLine(success ? "Payment succeeded (BLL)." : "Payment failed or was declined (BLL).");
        }
        private static async Task GenerateMonthlyReportAsync(IReportService? reportService)
        {
            Console.WriteLine("\n--- Monthly Sales Report ---");
            if (reportService == null)
            {
                Console.WriteLine("Report service is not available.");
                return;
            }

            var from = ReadDate("From date (yyyy-MM-dd): ", DateTime.UtcNow.AddMonths(-1));
            var to = ReadDate("To date (yyyy-MM-dd): ", DateTime.UtcNow);
            var reports = await reportService.GetMonthlySalesReportAsync(from, to);
            if (reports == null || !reports.Any())
            {
                Console.WriteLine("No report data available for the selected period.");
                return;
            }

            Console.WriteLine($"Report entries: {reports.Count()}");
            foreach (var r in reports)
            {
                Console.WriteLine($" Date: {r.SaleDate:yyyy-MM-dd} Orders: {r.TotalOrders} Units: {r.TotalUnitsSold} Revenue: {r.TotalRevenue:C}");
            }
        }

        private static async Task EnsureProductsPresentAsync(IProductService productService)
        {
            var existing = await productService.GetAvailableProductsAsync();
            if (existing.Any()) return;

            Console.WriteLine("No products found. If your ProductService supports adding products via BLL, please use 'Manage Products' to add them.");
        }

        private static void ConfigureAppServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseInMemoryDatabase("ECommerceDemoDB")
            );

            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // BLL services — Presentation interacts only with these
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService,ProductService>();
            services.AddScoped<IOrderService,OrderService>();
            services.AddScoped<IReportService,ReportService>();
        }

        #region Helpers
        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out var v)) return v;
                Console.WriteLine("Please enter a valid integer.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (decimal.TryParse(s, out var v)) return v;
                Console.WriteLine("Please enter a valid decimal number.");
            }
        }

        private static DateTime ReadDate(string prompt, DateTime defaultValue)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            if (DateTime.TryParse(s, out var d)) return d;
            Console.WriteLine("Invalid date, using default.");
            return defaultValue;
        }

        private static string Prompt(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }
        #endregion
    }
}