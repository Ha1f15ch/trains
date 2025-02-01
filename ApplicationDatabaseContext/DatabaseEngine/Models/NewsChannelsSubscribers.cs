using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public User User { get; set; }
        public NewsChannel NewsChannel { get; set; }
    }
}
