using AutoMapper;
using Data.Structure;
using Models;

namespace Services.Common
{
    public class SubscriptionMappingProfile : Profile
    {
        public SubscriptionMappingProfile()
        {
            // Subscription Plan mappings
            CreateMap<SubscriptionPlan, SubscriptionPlanDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PriceMonthly, opt => opt.MapFrom(src => (decimal)src.PriceMonthly))
                .ForMember(dest => dest.PriceYearly, opt => opt.MapFrom(src => (decimal)src.PriceYearly))
                .ForMember(dest => dest.AllowedChatRooms, opt => opt.MapFrom(src => src.AllowedChatRooms))
                .ForMember(dest => dest.AllowedFiles, opt => opt.MapFrom(src => src.AllowedFiles))
                .ForMember(dest => dest.AllowedFileSizeMb, opt => opt.MapFrom(src => src.AllowedFileSizeMb))
                .ForMember(dest => dest.Features, opt => opt.Ignore()); // Features are loaded separately

            // User Subscription mappings
            CreateMap<UserSubscription, UserSubscriptionDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => src.PlanId.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.PeriodType));

            // DTO to Entity mappings (for creating/updating)
            CreateMap<CreateSubscriptionRequest, UserSubscription>()
                .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => long.Parse(src.PlanId)))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.PeriodType == SubscriptionPeriodType.Monthly ? "Monthly" : "Yearly"))
                .ForMember(dest => dest.AutoRenew, opt => opt.MapFrom(src => src.AutoRenew))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.Ignore())
                .ForMember(dest => dest.EndDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.LastInvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.LastRenewalDate, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedByUserId, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Invoice to Financial Transaction mapping
            CreateMap<Invoice, FinancialTransaction>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => src.PlanId.ToString()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (decimal)src.Amount))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => (decimal)src.DiscountAmount))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => (decimal)src.TotalAmount))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<TransactionType>(src.Type)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<TransactionStatus>(src.Status)));

            // Discount Coupon mappings
            CreateMap<DiscountCoupon, DiscountCouponDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => (DiscountType)src.DiscountType))
                .ForMember(dest => dest.DiscountValue, opt => opt.MapFrom(src => (decimal)src.DiscountValue))
                .ForMember(dest => dest.ApplicablePlanIds, opt => opt.Ignore()); // Loaded separately
        }
    }
}
