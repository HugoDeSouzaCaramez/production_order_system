using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Models
{
    public class CreateProductionOrderDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int QuantityPlanned { get; set; }
        public ProductionOrderStatusEnum Status { get; set; } = ProductionOrderStatusEnum.Planejada;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}
