using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    [Comment("Separate Distribution Center with big Volume")]
    public class DistributionCenter : BaseDivision
    {
        public int StorageType { get; set; }
    }
}
