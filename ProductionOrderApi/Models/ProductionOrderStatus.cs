namespace ProductionOrderApi.Models
{
    public static class ProductionOrderStatus
    {
        public const string Planned = "Planejada";
        public const string InProgress = "EmProducao";
        public const string Finished = "Finalizada";

        public static readonly string[] All = new[]
        {
            Planned,
            InProgress,
            Finished
        };

        public static bool IsValid(string status)
        {
            return All.Contains(status);
        }
    }
}
