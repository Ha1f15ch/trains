using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.BookCommand.Queries
{
	public class GetAllBooksQuery : IRequest<List<Book>>
	{
	}
}
