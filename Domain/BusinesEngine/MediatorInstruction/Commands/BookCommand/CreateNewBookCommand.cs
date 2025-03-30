using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.BookCommand
{
	public class CreateNewBookCommand : IRequest<Book?>
	{
		public string Title { get; set; }
		public bool IsActive { get; set; }
		public string? Description { get; set; }
		public string? Author { get; set; }
		public int? CountList { get; set; }
		public string? CreatedAt { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}
