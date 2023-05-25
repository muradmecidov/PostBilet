using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; } 
        [Required]
        public string ImagePath { get; set; }
        public bool IsDeleted { get; set; } = default;
    }
}
