using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelPost.Queries
{
	public class GetAllPostsByPartTitleQuery : IRequest<List<NewsChannelsPosts>>
	{
		required public string PartNewsChannelPostsName { get; set; }
	}
}
