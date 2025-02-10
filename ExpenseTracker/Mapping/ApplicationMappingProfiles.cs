using AutoMapper;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Mapping;

public class ApplicationMappingProfiles : Profile
{
    public ApplicationMappingProfiles()
    {
        CreateMap<Expense, ExpenseDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<CreateExpenseDto, Expense>();
        CreateMap<UpdateExpenseDto, Expense>()
           .ForMember(dest => dest.CategoryId, opt =>
               opt.Condition(src => src.CategoryId.HasValue))
           .ForMember(dest => dest.Amount, opt =>
               opt.Condition(src => src.Amount.HasValue))
           .ForMember(dest => dest.Date, opt =>
               opt.Condition(src => src.Date.HasValue))
           .ForMember(dest => dest.Description, opt =>
               opt.Condition(src => !string.IsNullOrEmpty(src.Description)));
    }
}
