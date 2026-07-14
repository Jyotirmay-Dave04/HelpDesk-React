using AutoMapper;
using HelpdeskSystem.Common.DTOs.AuditLogs;
using HelpdeskSystem.Common.DTOs.CannedResponses;
using HelpdeskSystem.Common.DTOs.Categories;
using HelpdeskSystem.Common.DTOs.Comments;
using HelpdeskSystem.Common.DTOs.Groups;
using HelpdeskSystem.Common.DTOs.Notifications;
using HelpdeskSystem.Common.DTOs.SlaPolicies;
using HelpdeskSystem.Common.DTOs.SubCategories;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.DTOs.User;
using HelpdeskSystem.Domain.Entities;

namespace HelpdeskSystem.Application.Mappings;

public class AutoMappingProfile: Profile
{
    public AutoMappingProfile()
    {
        // User Mapping
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));


        // Group Mapping
        CreateMap<Group, GroupDto>();


        // Category Mapping
        CreateMap<Category, CategoryDto>();


        // Subcategory Mapping
        CreateMap<SubCategory, SubCategoryDto>();


        // Ticket Mapping
        CreateMap<Ticket, TicketResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : ""))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
            .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : ""))
            .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.RequestedByUser != null ? src.RequestedByUser.Name : ""))
            .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.Name : null))
            .ForMember(dest => dest.RequestedById, opt => opt.MapFrom(src => src.RequestedBy))
            .ForMember(dest => dest.AssignedToId, opt => opt.MapFrom(src => src.AssignedTo))
            .ForMember(dest => dest.IsSlaPaused, opt => opt.MapFrom(src => src.SlaPausedAt != null));
        
        CreateMap<Ticket, TicketListItemDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : ""))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
            .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : ""))
            .ForMember(dest => dest.RequesterName, opt => opt.MapFrom(src => src.RequestedByUser != null ? src.RequestedByUser.Name : ""))
            .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => src.AssignedToUser != null ? src.AssignedToUser.Name : null));
        

        // Audit Logs Mapping
        CreateMap<AuditLog, AuditLogResponseDto>()
            .ForMember(dest => dest.ChangedByName, opt => opt.MapFrom(src => src.ChangedByUser!.Name));
        

        // Comment Mapping
        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
        

        // Notification Mapping
        CreateMap<Notification, NotificationResponseDto>();


        // SLA Policy Mapping
        CreateMap<SlaPolicy, SlaPolicyResponseDto>()
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()));
        

        // Canned Response Mapping
        CreateMap<CannedResponse, CannedResponseResponseDto>();
    }
}
