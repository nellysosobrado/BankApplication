using AutoMapper;
using BankApplication.ViewModels;
using DAL.Models;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Givenname} {src.Surname}"))
                .ForMember(dest => dest.Personnummer, opt => opt.MapFrom(src => src.NationalId ?? "N/A"))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Streetaddress))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Telephonenumber ?? "N/A"))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId.ToString()));

            CreateMap<Customer, CustomerSummaryViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Givenname + " " + src.Surname))
                .ForMember(dest => dest.AccountCount, opt => opt.MapFrom(src =>
                    src.Dispositions.Count(d => d.Account != null)))
                .ForMember(dest => dest.TotalBalance, opt => opt.MapFrom(src =>
                    src.Dispositions.Where(d => d.Account != null).Sum(d => d.Account.Balance)));


        }
    }
}
