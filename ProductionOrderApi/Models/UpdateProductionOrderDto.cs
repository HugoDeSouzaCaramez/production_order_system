using ProductionOrderApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProductionOrderApi.Models
{
    public class UpdateProductionOrderDto
    {
        public string? OrderNumber { get; set; }
        public string? ProductCode { get; set; }
        public int? QuantityPlanned { get; set; }
        public ProductionOrderStatusEnum? Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public bool HasDateConflict()
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                return StartDate.Value.Date > EndDate.Value.Date;
            }
            return false;
        }
    }
}
