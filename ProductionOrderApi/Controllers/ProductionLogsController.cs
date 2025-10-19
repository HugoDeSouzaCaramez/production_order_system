using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionOrderApi.Models;
using ProductionOrderApi.Services;

namespace ProductionOrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionLogsController : ControllerBase
    {
        private readonly IProductionLogService _logService;

        public ProductionLogsController(IProductionLogService logService)
        {
            _logService = logService;
        }

        [HttpPost]
        public async Task<ActionResult<ProductionLog>> CreateProductionLog(CreateProductionLogDto logDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (logDto.Quantity <= 0)
                    return BadRequest("Quantidade deve ser maior que zero");

                if (logDto.ProductionOrderId <= 0)
                    return BadRequest("ID da ordem é obrigatório e deve ser maior que zero");

                var log = new ProductionLog
                {
                    ProductionOrderId = logDto.ProductionOrderId,
                    ResourceId = logDto.ResourceId,
                    Quantity = logDto.Quantity
                };

                var createdLog = await _logService.AddProductionLogAsync(log);
                return CreatedAtAction(nameof(GetLog), new { id = createdLog.Id }, createdLog);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                
                return Conflict(new { message = "Erro ao criar log de produção. Verifique os dados inseridos." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(503, new { message = "Serviço temporariamente indisponível" });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionLog>>> GetLogs()
        {
            try
            {
                var logs = await _logService.GetAllProductionLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogs: {ex.Message}");
                return StatusCode(503, new { message = "Serviço temporariamente indisponível" });
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<ProductionLog>>> GetLogsByOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest(new { message = "ID da ordem deve ser maior que zero" });

            try
            {
                var logs = await _logService.GetProductionLogsByOrderAsync(orderId);

                if (logs == null || !logs.Any())
                    return NotFound(new { message = $"Nenhum log encontrado para a ordem {orderId}" });

                return Ok(logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLogsByOrder: {ex.Message}");
                return StatusCode(503, new { message = "Serviço temporariamente indisponível" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionLog>> GetLog(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID deve ser maior que zero" });

            try
            {
                var log = await _logService.GetProductionLogByIdAsync(id);

                if (log == null)
                    return NotFound(new { message = $"Log com ID {id} não encontrado" });

                return Ok(log);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetLog: {ex.Message}");
                return StatusCode(503, new { message = "Serviço temporariamente indisponível" });
            }
        }
    }
}
