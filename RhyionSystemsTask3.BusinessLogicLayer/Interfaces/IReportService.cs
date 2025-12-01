using RhyionSystemsTask3.BusinessLogicLayer.Services;


namespace RhyionSystemsTask3.BusinessLogicLayer.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<MonthlySalesReportDto>> GetMonthlySalesReportAsync(DateTime startDate, DateTime endDate);
    }
}
