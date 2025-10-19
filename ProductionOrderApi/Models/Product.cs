namespace ProductionOrderApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
    }
}
