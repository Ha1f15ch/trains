using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseEngine.Models
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        public User()
        {
            Subscriptions = new List<Subscription>();
            ListNewsChannelsSubscribers = new List<NewsChannelsSubscribers>();
        }

        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool UserIsActive { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime? DateDelete { get; set; }

		[JsonIgnore]
		public List<Subscription> Subscriptions { get; set; }
		[JsonIgnore]
		public List<NewsChannelsSubscribers> ListNewsChannelsSubscribers { get; set; }
    }
}
