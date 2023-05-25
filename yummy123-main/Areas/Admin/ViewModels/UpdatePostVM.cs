using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Areas.Admin.ViewModels
{
	public class UpdatePostVM
	{
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		public string Title { get; set; }
		[Required]
		[MaxLength(255)]
		public string Description { get; set; }
		[Required]
		public IFormFile Photo { get; set; }
	}
}
