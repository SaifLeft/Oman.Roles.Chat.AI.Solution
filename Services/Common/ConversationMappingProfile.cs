using AutoMapper;
using Data.Structure;
using Models;
using System.Text.Json;

namespace Services.Common
{
    public class ConversationMappingProfile : Profile
    {
        public ConversationMappingProfile()
        {
            // Map from entity to DTO
            CreateMap<ConversationTracking, ConversationTrackingDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src =>
                        !string.IsNullOrEmpty(src.MetadataJson)
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(src.MetadataJson, new JsonSerializerOptions())
                        : new Dictionary<string, object>()))
                .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.ConversationKeywords))
                .ForMember(dest => dest.PdfReferences, opt => opt.MapFrom(src => src.ConversationPdfReferences));

            // Map related entities
            CreateMap<ConversationKeyword, ConversationKeywordDTO>();

            CreateMap<ConversationPdfReference, ConversationPdfReferenceDTO>()
                .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.FileId.ToString()));
        }
    }
}