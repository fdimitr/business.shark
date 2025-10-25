namespace BusinessSharkClient.Logic.Models
{
    public class ComponentUnitModel
    {
        public int ProductDefinitionId { get; set; }
        public int ComponentDefinitionId { get; set; }
        public double ProductionQuantity { get; set; }
        public double QualityImpact { get; set; }
    }
}
