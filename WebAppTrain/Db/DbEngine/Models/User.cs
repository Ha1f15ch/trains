using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbEngine.Models
{
	[Table("User", Schema = "dbo")]
	public class User
	{
		public User()
		{
			
		}
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateDelete { get; set; }
	}
}
