using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseEngine.Models
{
    [Table("NewsChannel", Schema = "dict")]
    public class NewsChannel
    {
        public NewsChannel()
        {
            ListNewsChannelsPosts = new List<NewsChannelsPosts>();
            ListNewsChannelsSubscribers = new List<NewsChannelsSubscribers>();
        }

        [Key] public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int CountSubscribers { get; set; }
        public DateTime DateCreated { get; set; }

		[JsonIgnore]
		public List<NewsChannelsPosts> ListNewsChannelsPosts { get; set; }
		[JsonIgnore]
		public List<NewsChannelsSubscribers> ListNewsChannelsSubscribers { get; set; }
    }
}
