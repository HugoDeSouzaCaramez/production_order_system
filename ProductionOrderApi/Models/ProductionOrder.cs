using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Models
{
    public class ProductionOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int QuantityPlanned { get; set; }
        public int QuantityProduced { get; set; }
        public ProductionOrderStatusEnum Status { get; set; } = ProductionOrderStatusEnum.Planejada;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<ProductionLog> ProductionLogs { get; set; } = new List<ProductionLog>();
    }
}
