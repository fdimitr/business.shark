using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class EmployeesEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public int DivisionId { get; set; }
        public int TotalQuantity { get; set; }
        public double SkillLevel { get; set; }
        public double Salary { get; set; }

        public object[] GetKeyValues() => [Id];
    }
}
