using ProductionOrderApi.Models;

namespace ProductionOrderApi.Services
{
    public interface IProductionLogService
    {
        Task<ProductionLog> AddProductionLogAsync(ProductionLog log);
        Task<IEnumerable<ProductionLog>> GetAllProductionLogsAsync();
        Task<IEnumerable<ProductionLog>> GetProductionLogsByOrderAsync(int orderId);
        Task<ProductionLog?> GetProductionLogByIdAsync(int id);
    }
}
