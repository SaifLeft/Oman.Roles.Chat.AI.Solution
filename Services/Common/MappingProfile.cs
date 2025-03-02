using AutoMapper;
using Data.Structure;
using Models;

namespace Services.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => new List<string> { src.UserRole }));

            CreateMap<ChatRoom, ChatRoomDTO>();
            CreateMap<ChatMessage, ChatMessageDTO>();
            CreateMap<SubscriptionPlan, SubscriptionPlanDTO>();
            CreateMap<UserSubscription, UserSubscriptionDTO>();
            CreateMap<DataSourceFile, DataSourceFileDTO>()
                .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src =>
                    src.DataSourceFileKeywords.Select(k => k.Keyword).ToList()));

            // DTO to Entity mappings
            CreateMap<RegisterUserRequestDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore());

            CreateMap<UpdateFileInfoRequestDTO, DataSourceFile>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}