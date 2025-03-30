using AutoMapper;
using DatabaseEngine.Models;
using DTOs;

namespace MapperService
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Новостные каналы
			CreateMap<NewsChannelDto, NewsChannel>()
				.ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.CountSubscribers, opt => opt.MapFrom(src => 0))
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.ListNewsChannelsPosts, opt => opt.Ignore())
				.ForMember(dest => dest.ListNewsChannelsSubscribers, opt => opt.Ignore());
		}
	}
}
