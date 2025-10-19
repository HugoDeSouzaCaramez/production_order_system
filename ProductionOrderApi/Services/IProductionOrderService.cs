using ProductionOrderApi.Models;

namespace ProductionOrderApi.Services
{
    public interface IProductionOrderService
    {
        Task<IEnumerable<ProductionOrder>> GetAllOrdersAsync();
        Task<ProductionOrder?> GetOrderByIdAsync(int id);
        Task<ProductionOrder> CreateOrderAsync(ProductionOrder order);
        Task<ProductionOrder> UpdateOrderAsync(int id, ProductionOrder order);
        Task<IEnumerable<ProductionOrder>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<string>> GetPossibleStatusesAsync();
    }
}
