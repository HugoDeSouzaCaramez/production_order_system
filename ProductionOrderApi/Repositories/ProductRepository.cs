using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Data;
using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Product
                .Include(p => p.ProductionOrders)
                .ToListAsync();
        }
        
        public async Task<Product?> GetByCodeAsync(string code)
        {
            return await _context.Product
                .FirstOrDefaultAsync(p => p.Code == code);
        }
    }
}
