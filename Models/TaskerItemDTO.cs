using System.ComponentModel.DataAnnotations;

namespace TaskerBCAPI.Models
{
    public class TaskerItemDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }

        public bool? IsComplete { get; set; }
    }
}
