using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByCodeAsync(string code);
    }
}
