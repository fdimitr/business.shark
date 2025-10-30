using BusinessSharkService.DataAccess.Models.Divisions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models
{
    [Comment("Represents workers involved in production, affecting quantity and quality based on technology level.")]
    public class Employees
    {
        [Key]
        public int EmployeesId { get; set; }

        public int DivisionId { get; set; }
        public Division? Division { get; set; }

        [Comment("The total number of workers involved in production.")]
        public int TotalQuantity { get; set; }

        [Comment("Technology level of the workers, influencing their effectiveness.")]
        public double TechLevel { get; set; } = 1;

        public double SalaryPerEmployee { get; set; }

        }
}
