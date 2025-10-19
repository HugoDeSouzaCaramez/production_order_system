using ProductionOrderApi.Models;
using ProductionOrderApi.Repositories;

namespace ProductionOrderApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Code = p.Code,
                Description = p.Description
            });
        }
    }
}
