using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Data;
using ProductionOrderApi.Enums;
using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public class ProductionOrderRepository : IProductionOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductionOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductionOrder>> GetAllAsync()
        {
            return await _context.ProductionOrder
                .Include(po => po.Product)
                .Include(po => po.ProductionLogs)
                .ToListAsync();
        }

        public async Task<ProductionOrder?> GetByIdAsync(int id)
        {
            return await _context.ProductionOrder
                .Include(po => po.Product)
                .Include(po => po.ProductionLogs)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<ProductionOrder?> GetByOrderNumberAsync(string orderNumber, int? excludeId = null)
        {
            IQueryable<ProductionOrder> query = _context.ProductionOrder
                .Where(po => po.OrderNumber == orderNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(po => po.Id != excludeId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ProductionOrder> CreateAsync(ProductionOrder order)
        {
            _context.ProductionOrder.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<ProductionOrder> UpdateAsync(ProductionOrder order)
        {
            var existingOrder = await _context.ProductionOrder
                .FirstOrDefaultAsync(po => po.Id == order.Id);

            if (existingOrder == null)
                throw new KeyNotFoundException("Ordem não encontrada");

            _context.Entry(existingOrder).CurrentValues.SetValues(new
            {
                order.OrderNumber,
                order.ProductCode,
                order.QuantityPlanned,
                order.Status,
                order.StartDate,
                order.EndDate
            });

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.ProductionOrder.FindAsync(id);
            if (order == null)
                return false;

            _context.ProductionOrder.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductionOrder>> GetByStatusAsync(string status)
        {
            if (!Enum.TryParse<ProductionOrderStatusEnum>(status, out var statusEnum))
            {
                throw new ArgumentException($"Status '{status}' é inválido");
            }

            return await _context.ProductionOrder
                .Include(po => po.Product)
                .Include(po => po.ProductionLogs)
                .Where(po => po.Status == statusEnum)
                .ToListAsync();
        }
    }
}
