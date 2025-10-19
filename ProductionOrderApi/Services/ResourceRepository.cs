using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Data;
using ProductionOrderApi.Models;

namespace ProductionOrderApi.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly ApplicationDbContext _context;

        public ResourceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Resource.AnyAsync(r => r.Id == id);
        }

        public async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resource.FindAsync(id);
        }
    }
}
