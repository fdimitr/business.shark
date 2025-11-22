using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class CountryEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(30)]
        public required string Name { get; set; }

        [Required]
        [StringLength(2)]
        public required string Code { get; set; }
    }
}
