using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseEngine.Models
{
    [Table("NewsChannelsSubscribers", Schema = "dbo")]
    public class NewsChannelsSubscribers
    {
        public NewsChannelsSubscribers()
        {
            
        }

        [Key]
        public int Id { get; set; }
        public required int NewsChannelId { get; set; }
        public required int UserId { get; set; }
        public DateTime CreatedDate { get; set; }

		[JsonIgnore]
		public User User { get; set; }
		[JsonIgnore]
		public NewsChannel NewsChannel { get; set; }
    }
}
