// Exceptions/NotFoundException.cs
using AutoMapper;
using SportTrack_v1.Controladores.Bote.Dtos;
using SportTrack_v1.Controladores.Categoria.Dtos;
using SportTrack_v1.Controladores.Distancia.Dtos;
using SportTrack_v1.Entidades.Entidades;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Extensions/EnumExtensions.cs
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }
}

// Profile para AutoMapper
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bote, BoteDto>().ReverseMap();
        CreateMap<BoteCreateDto, Bote>();
        CreateMap<BoteUpdateDto, Bote>();

        CreateMap<Categoria, CategoriaDto>().ReverseMap();
        CreateMap<CategoriaCreateDto, Categoria>();
        CreateMap<CategoriaUpdateDto, Categoria>();

        CreateMap<Distancia, DistanciaDto>()
            .ForMember(dest => dest.DistanciaRegata, opt => opt.MapFrom(src => (int)src.DistanciaRegata));

        CreateMap<DistanciaCreateDto, Distancia>();
        CreateMap<DistanciaUpdateDto, Distancia>();
    }
}