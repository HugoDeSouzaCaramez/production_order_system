using ProductionOrderApi.Models;

namespace ProductionOrderApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}
