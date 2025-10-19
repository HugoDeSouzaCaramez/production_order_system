using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public interface IProductionOrderRepository
    {
        Task<IEnumerable<ProductionOrder>> GetAllAsync();
        Task<ProductionOrder?> GetByIdAsync(int id);
        Task<ProductionOrder?> GetByOrderNumberAsync(string orderNumber, int? excludeId = null);
        Task<ProductionOrder> CreateAsync(ProductionOrder order);
        Task<ProductionOrder> UpdateAsync(ProductionOrder order);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProductionOrder>> GetByStatusAsync(string status);
    }
}
