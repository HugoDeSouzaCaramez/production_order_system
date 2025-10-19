using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public interface IResourceRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<Resource?> GetByIdAsync(int id);
    }
}
