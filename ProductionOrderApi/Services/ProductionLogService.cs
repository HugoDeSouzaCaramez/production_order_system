using ProductionOrderApi.Models;
using ProductionOrderApi.Repositories;

namespace ProductionOrderApi.Services
{
    public class ProductionLogService : IProductionLogService
    {
        private readonly IProductionLogRepository _logRepository;
        private readonly IProductionOrderRepository _orderRepository;
        private readonly IResourceRepository _resourceRepository;

        public ProductionLogService(
            IProductionLogRepository logRepository,
            IProductionOrderRepository orderRepository,
            IResourceRepository resourceRepository)
        {
            _logRepository = logRepository;
            _orderRepository = orderRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<ProductionLog> AddProductionLogAsync(ProductionLog log)
        {
            var order = await _orderRepository.GetByIdAsync(log.ProductionOrderId);
            if (order == null)
                throw new ArgumentException($"Ordem de produção com ID {log.ProductionOrderId} não encontrada");

            if (log.ResourceId.HasValue)
            {
                var resourceExists = await _resourceRepository.ExistsAsync(log.ResourceId.Value);
                if (!resourceExists)
                    throw new ArgumentException($"Recurso com ID {log.ResourceId.Value} não encontrado");
            }

            if (log.Quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");

            if (order.QuantityProduced + log.Quantity > order.QuantityPlanned)
                throw new InvalidOperationException(
                    $"Quantidade excede o planejado. " +
                    $"Planejado: {order.QuantityPlanned}, " +
                    $"Já produzido: {order.QuantityProduced}, " +
                    $"Tentando adicionar: {log.Quantity}");

            var createdLog = await _logRepository.AddAsync(log);

            order.QuantityProduced += log.Quantity;
            await _orderRepository.UpdateAsync(order);

            return createdLog;
        }

        public async Task<IEnumerable<ProductionLog>> GetAllProductionLogsAsync()
        {
            return await _logRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ProductionLog>> GetProductionLogsByOrderAsync(int orderId)
        {
            return await _logRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<ProductionLog?> GetProductionLogByIdAsync(int id)
        {
            return await _logRepository.GetByIdAsync(id);
        }
    }
}
