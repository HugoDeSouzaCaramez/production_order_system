using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Models;
using ProductionOrderApi.Services;
using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IProductionOrderService _orderService;

        public OrdersController(IProductionOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionOrder>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionOrder>> GetOrder(int id)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero");

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound($"Ordem com ID {id} não encontrada");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductionOrder>> CreateOrder(CreateProductionOrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = new ProductionOrder
                {
                    OrderNumber = orderDto.OrderNumber.Trim(),
                    ProductCode = orderDto.ProductCode.Trim(),
                    QuantityPlanned = orderDto.QuantityPlanned,
                    Status = orderDto.Status,
                    StartDate = orderDto.StartDate,
                    QuantityProduced = 0
                };

                var createdOrder = await _orderService.CreateOrderAsync(order);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return Conflict("Erro ao criar ordem. Verifique os dados inseridos.");
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateProductionOrderDto orderDto)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (orderDto.HasDateConflict())
            {
                return BadRequest(new
                {
                    message = "Data de início não pode ser maior que data de término"
                });
            }

            try
            {
                var existingOrder = await _orderService.GetOrderByIdAsync(id);
                if (existingOrder == null)
                    return NotFound($"Ordem com ID {id} não encontrada");

                if (!string.IsNullOrEmpty(orderDto.OrderNumber))
                    existingOrder.OrderNumber = orderDto.OrderNumber;

                if (!string.IsNullOrEmpty(orderDto.ProductCode))
                    existingOrder.ProductCode = orderDto.ProductCode;

                if (orderDto.QuantityPlanned.HasValue)
                    existingOrder.QuantityPlanned = orderDto.QuantityPlanned.Value;

                if (orderDto.Status.HasValue)
                    existingOrder.Status = orderDto.Status.Value;

                if (orderDto.StartDate.HasValue)
                    existingOrder.StartDate = orderDto.StartDate.Value;

                if (orderDto.EndDate.HasValue)
                    existingOrder.EndDate = orderDto.EndDate.Value;

                var updatedOrder = await _orderService.UpdateOrderAsync(id, existingOrder);
                return Ok(updatedOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Ordem com ID {id} não encontrada");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return Conflict("Erro ao criar ordem. Verifique os dados inseridos.");
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProductionOrder>>> GetOrdersByStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Status é obrigatório");

            if (!Enum.TryParse<ProductionOrderStatusEnum>(status, out _))
            {
                return BadRequest($"Status '{status}' é inválido. Status válidos: {string.Join(", ", Enum.GetNames(typeof(ProductionOrderStatusEnum)))}");
            }

            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);

                if (orders == null || !orders.Any())
                    return NotFound($"Nenhuma ordem encontrada com status '{status}'");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }

        [HttpGet("status/possible")]
        public async Task<ActionResult<IEnumerable<string>>> GetPossibleStatuses()
        {
            try
            {
                var statuses = await _orderService.GetPossibleStatusesAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(503, "Serviço temporariamente indisponível");
            }
        }
    }
}
