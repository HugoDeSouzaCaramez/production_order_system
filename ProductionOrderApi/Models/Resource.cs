using ProductionOrderApi.Enums;

namespace ProductionOrderApi.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceStatusEnum Status { get; set; } = ResourceStatusEnum.Disponivel;

        public ICollection<ProductionLog> ProductionLogs { get; set; } = new List<ProductionLog>();
    }
}
