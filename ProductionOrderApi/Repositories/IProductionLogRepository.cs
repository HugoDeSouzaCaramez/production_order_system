using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public interface IProductionLogRepository
    {
        Task<ProductionLog> AddAsync(ProductionLog log);
        Task<IEnumerable<ProductionLog>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<ProductionLog>> GetAllAsync();
        Task<ProductionLog?> GetByIdAsync(int id);
    }
}
