using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RhyionSystemsTask3.BusinessLogicLayer.Interfaces;
using RhyionSystemTask3.DataAccessLayer.Context;
using RhyionSystemTask3.DataAccessLayer.UnitOfWork;

namespace RhyionSystemsTask3.BusinessLogicLayer.Services
{
    public class MonthlySalesReportDto
    {
        public DateTime SaleDate { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ReportService : IReportService
    {
        private readonly AppDBContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public ReportService(AppDBContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MonthlySalesReportDto>> GetMonthlySalesReportAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            var sql = "EXEC Monthly_Sales_Report @StartDate, @EndDate";

            var startParam = new SqlParameter("@StartDate", startDate.Date);
            var endParam = new SqlParameter("@EndDate", endDate.Date);

            var results = await _context.Set<MonthlySalesReportDto>()
                .FromSqlRaw(sql, startParam, endParam)
                .ToListAsync();

            return results;
        }
    }
}
