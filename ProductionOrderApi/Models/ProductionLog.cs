namespace ProductionOrderApi.Models
{
    public class ProductionLog
    {
        public int Id { get; set; }
        public int ProductionOrderId { get; set; }
        public int? ResourceId { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ProductionOrder ProductionOrder { get; set; } = null!;
        public Resource? Resource { get; set; }
    }
}
