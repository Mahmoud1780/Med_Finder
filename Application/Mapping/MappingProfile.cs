using AutoMapper;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Domain.Entities;

namespace MedicineFinder.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, ReservationDto>();
        CreateMap<Medicine, MedicineAlternativeDto>()
            .ForMember(dest => dest.MedicineId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Stock, StockDto>()
            .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine != null ? src.Medicine.Name : string.Empty));
        CreateMap<Pharmacy, PharmacyDto>();
    }
}
