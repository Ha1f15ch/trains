using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DatabaseEngine.Models
{
    [Table("Subscription", Schema = "dbo")]
    public class Subscription
    {
        public Subscription()
        {
            
        }

        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime SubscriptionDate { get; set; }

		[JsonIgnore]
		public User User { get; set; }
		[JsonIgnore]
		public Book Book { get; set; }
    }
}
