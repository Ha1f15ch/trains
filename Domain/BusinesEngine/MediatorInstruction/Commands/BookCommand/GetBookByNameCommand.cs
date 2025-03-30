using DatabaseEngine.Models;
using MediatR;

namespace BusinesEngine.MediatorInstruction.Commands.BookCommand
{
	public class GetBookByNameCommand : IRequest<List<Book?>>
	{
		public string PartTitleName { get; set; }
	}
}
