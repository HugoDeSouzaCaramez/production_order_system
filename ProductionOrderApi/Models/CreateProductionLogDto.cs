namespace ProductionOrderApi.Models
{
    public class CreateProductionLogDto
    {
        public int ProductionOrderId { get; set; }
        public int? ResourceId { get; set; }
        public int Quantity { get; set; }
    }
}
