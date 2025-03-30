using DatabaseEngine.Models;
using MediatR;

namespace BusinesEngine.Commands.BookCommand
{
	public class GetBookByNameCommand : IRequest<List<Book?>>
	{
		public string PartTitleName { get; set; }
	}
}
