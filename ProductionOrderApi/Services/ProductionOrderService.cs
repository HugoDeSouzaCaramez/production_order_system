using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Models;
using ProductionOrderApi.Repositories;
using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Services
{
    public class ProductionOrderService : IProductionOrderService
    {
        private readonly IProductionOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public ProductionOrderService(IProductionOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<string>> GetPossibleStatusesAsync()
        {
            return await Task.FromResult(Enum.GetNames(typeof(ProductionOrderStatusEnum)));
        }

        public async Task<IEnumerable<ProductionOrder>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<ProductionOrder?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID deve ser maior que zero");

            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<ProductionOrder> CreateOrderAsync(ProductionOrder order)
        {
            if (string.IsNullOrEmpty(order.OrderNumber))
                throw new ArgumentException("Número da ordem é obrigatório");

            if (string.IsNullOrEmpty(order.ProductCode))
                throw new ArgumentException("Código do produto é obrigatório");

            if (order.QuantityPlanned <= 0)
                throw new ArgumentException("Quantidade planejada deve ser maior que zero");

            var existingOrder = await _orderRepository.GetByOrderNumberAsync(order.OrderNumber);
            if (existingOrder != null)
            {
                throw new InvalidOperationException(
                    $"Já existe uma ordem de produção com o número '{order.OrderNumber}'");
            }
            
            var productExists = await _productRepository.GetByCodeAsync(order.ProductCode);
            if (productExists == null)
            {
                throw new InvalidOperationException(
                    $"Código de produto '{order.ProductCode}' não existe");
            }

            order.QuantityProduced = 0;

            if (order.Status == 0)
            {
                order.Status = ProductionOrderStatusEnum.Planejada;
            }

            if (order.Status == ProductionOrderStatusEnum.Finalizada)
            {
                var startDateOnly = order.StartDate.Date;
                var endDateOnly = DateTime.UtcNow.Date;

                if (startDateOnly > endDateOnly)
                {
                    throw new InvalidOperationException(
                        $"Não é possível finalizar ordem: data de início ({startDateOnly:dd/MM/yyyy}) é maior que data atual ({endDateOnly:dd/MM/yyyy})");
                }

                order.EndDate = DateTime.UtcNow;
            }

            return await _orderRepository.CreateAsync(order);
        }

        public async Task<ProductionOrder> UpdateOrderAsync(int id, ProductionOrder order)
        {
            if (id <= 0)
                throw new ArgumentException("ID deve ser maior que zero");

            if (id != order.Id)
                throw new ArgumentException("ID não corresponde");

            var duplicateOrder = await _orderRepository.GetByOrderNumberAsync(order.OrderNumber, id);
            if (duplicateOrder != null)
            {
                throw new InvalidOperationException(
                    $"Já existe uma ordem de produção com o número '{order.OrderNumber}'");
            }
            
            var productExists = await _productRepository.GetByCodeAsync(order.ProductCode);
            if (productExists == null)
            {
                throw new InvalidOperationException(
                    $"Código de produto '{order.ProductCode}' não existe");
            }

            if (order.Status == ProductionOrderStatusEnum.Planejada && order.EndDate != null)
            {
                order.EndDate = null;
            }

            if (order.Status == ProductionOrderStatusEnum.Finalizada)
            {
                var startDateOnly = order.StartDate.Date;
                var endDateOnly = DateTime.UtcNow.Date;

                if (startDateOnly > endDateOnly)
                {
                    throw new InvalidOperationException(
                        $"Não é possível finalizar ordem: data de início ({startDateOnly:dd/MM/yyyy}) é maior que data atual ({endDateOnly:dd/MM/yyyy})");
                }

                order.EndDate = DateTime.UtcNow;
            }

            if (order.EndDate.HasValue)
            {
                var startDateOnly = order.StartDate.Date;
                var endDateOnly = order.EndDate.Value.Date;

                if (startDateOnly > endDateOnly)
                {
                    throw new ArgumentException(
                        $"Data de início ({startDateOnly:dd/MM/yyyy}) não pode ser maior que data de término ({endDateOnly:dd/MM/yyyy})");
                }
            }

            return await _orderRepository.UpdateAsync(order);
        }

        public async Task<IEnumerable<ProductionOrder>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Status é obrigatório");

            if (!Enum.TryParse<ProductionOrderStatusEnum>(status, out var statusEnum))
            {
                throw new ArgumentException($"Status '{status}' é inválido");
            }

            return await _orderRepository.GetByStatusAsync(status);
        }
    }
}
