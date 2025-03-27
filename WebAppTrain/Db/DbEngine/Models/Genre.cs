using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DbEngine.Models
{
	[Table("Genre", Schema = "dict")]
	public class Genre
	{
		public Genre()
		{
			
		}

		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateDelete { get; set; }

		[JsonIgnore]
		public List<BookGenre> BookGenres { get; set; }
	}
}
