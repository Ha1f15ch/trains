using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DbEngine.Models
{
	[Table("BookGenre", Schema = "dbo")]
	public class BookGenre
	{
		public BookGenre()
		{
			
		}

		[Key]
		public int Id { get; set; }
		public int BookId { get; set; }
		public int GenreId { get; set; }

		[JsonIgnore]
		public Book Book { get; set; }
		[JsonIgnore]
		public Genre Genre { get; set; }
	}
}
