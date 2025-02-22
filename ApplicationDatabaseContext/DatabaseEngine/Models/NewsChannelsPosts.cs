using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseEngine.Models
{
    [Table("NewsChannelsPosts", Schema = "dbo")]
    public class NewsChannelsPosts
    {
        public NewsChannelsPosts()
        {
            
        }

        [Key] public int Id { get; set; }
        public int NewsChannelId { get; set; }
        public required string TitlePost { get; set; }
        public required string BodyPost { get; set; }
        public string? FooterPost { get; set; }
        public required string AuthorPost { get;set; }
        public string? SurceImage { get; set; }
        public DateTime CreatedDate { get; set; }

		[JsonIgnore]
		public NewsChannel NewsChannel { get; set; }
    }
}
