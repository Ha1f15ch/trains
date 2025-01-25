using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.Models
{
    [Table("Book", Schema = "dbo")]
    public class Book
    {
        public Book()
        {
            
        }

        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required bool IsActive { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public int? CountLists { get; set; } = default(int);
        public string? CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
