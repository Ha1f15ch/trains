using DatabaseEngine.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinesEngine.MediatorInstruction.Commands.ChannelPost
{
	public class CreateNewNewsChannelsPostCommand : IRequest<NewsChannelsPosts?>
	{
		public int NewsChannelId { get; set; }
		public string TitlePost { get; set; }
		public string BodyPost { get; set; }
		public string? FooterPost { get; set; }
		public string AauthorPost { get; set; }
		public string? SourceImage { get; set; }
	}
}
