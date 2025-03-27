using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DbEngine.Models
{
	[Table("Book", Schema = "dict")]
	public class Book
	{
		public Book() { }

		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }

		[JsonIgnore]
		public List<BookGenre> BookGenres { get; set; }
	}
}
