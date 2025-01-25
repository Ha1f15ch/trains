using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public User User { get; set; }
        public Book Book { get; set; }
    }
}
