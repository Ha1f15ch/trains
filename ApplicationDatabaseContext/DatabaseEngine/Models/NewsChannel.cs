using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<NewsChannelsPosts> ListNewsChannelsPosts { get; set; }
        public List<NewsChannelsSubscribers> ListNewsChannelsSubscribers { get; set; }
    }
}
