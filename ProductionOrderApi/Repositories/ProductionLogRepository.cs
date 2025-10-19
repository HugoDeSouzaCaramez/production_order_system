using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Data;
using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public class ProductionLogRepository : IProductionLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductionLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductionLog> AddAsync(ProductionLog log)
        {
            _context.ProductionLog.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<IEnumerable<ProductionLog>> GetByOrderIdAsync(int orderId)
        {
            return await _context.ProductionLog
                .Where(pl => pl.ProductionOrderId == orderId)
                .Include(pl => pl.Resource)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductionLog>> GetAllAsync()
        {
            return await _context.ProductionLog
                .Include(pl => pl.ProductionOrder)
                .Include(pl => pl.Resource)
                .ToListAsync();
        }
        
        public async Task<ProductionLog?> GetByIdAsync(int id)
        {
            return await _context.ProductionLog
                .Include(pl => pl.ProductionOrder)
                .Include(pl => pl.Resource)
                .FirstOrDefaultAsync(pl => pl.Id == id);
        }
    }
}
