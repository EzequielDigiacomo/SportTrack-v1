using AutoMapper;
using SportTrack_v1.Controladores.Bote.Dtos;
using SportTrack_v1.Controladores.Categoria.Dtos;
using SportTrack_v1.Controladores.Distancia.Dtos;
using SportTrack_v1.Controladores.Inscripcion.Dtos;
using SportTrack_v1.Controladores.Evento.Dtos;
using SportTrack_v1.Controladores.Auth.Dtos;
using SportTrack_v1.Controladores.Club.Dtos;
using SportTrack_v1.Controladores.Participante.Dtos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Controladores.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeos de Club
            CreateMap<Entidades.Entidades.Club, ClubDto>()
                .ForMember(dest => dest.CantidadAtletas, opt => opt.MapFrom(src => src.Participantes != null ? src.Participantes.Count : 0))
                .ForMember(dest => dest.ParentClubNombre, opt => opt.MapFrom(src => src.ParentClub != null ? src.ParentClub.Nombre : null))
                .ForMember(dest => dest.PlanNombre, opt => opt.MapFrom(src => src.PlanSaaS != null ? src.PlanSaaS.Nombre : null))
                .ReverseMap();
            CreateMap<ClubCreateDto, Entidades.Entidades.Club>();
            CreateMap<ClubUpdateDto, Entidades.Entidades.Club>();

            // Mapeos de Bote
            CreateMap<Entidades.Entidades.Bote, BoteDto>().ReverseMap();
            CreateMap<BoteCreateDto, Entidades.Entidades.Bote>();
            CreateMap<BoteUpdateDto, Entidades.Entidades.Bote>();

            // Mapeos de Categoria
            CreateMap<Entidades.Entidades.Categoria, CategoriaDto>().ReverseMap();
            CreateMap<CategoriaCreateDto, Entidades.Entidades.Categoria>();
            CreateMap<CategoriaUpdateDto, Entidades.Entidades.Categoria>();

            // Mapeos de Distancia
            CreateMap<Entidades.Entidades.Distancia, DistanciaDto>()
                .ForMember(dest => dest.DistanciaRegata, opt => opt.MapFrom(src => (int)src.DistanciaRegata));
            CreateMap<DistanciaCreateDto, Entidades.Entidades.Distancia>();
            CreateMap<DistanciaUpdateDto, Entidades.Entidades.Distancia>();

            // Mapeos de Inscripciones
            CreateMap<InscripcionTripulante, InscripcionTripulanteDto>()
                .ForMember(dest => dest.ParticipanteNombreCompleto, opt => opt.MapFrom(src => src.Participante != null ? $"{src.Participante.Nombre} {src.Participante.Apellido}" : null));
            CreateMap<InscripcionTripulanteCreateDto, InscripcionTripulante>();

            CreateMap<Entidades.Entidades.Inscripcion, InscripcionDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.ParticipanteNombreCompleto, opt => opt.MapFrom(src => src.Participante != null ? $"{src.Participante.Nombre} {src.Participante.Apellido}" : null))
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Participante != null && src.Participante.Club != null ? src.Participante.Club.Nombre : null))
                .ForMember(dest => dest.ClubSigla, opt => opt.MapFrom(src => src.Participante != null && src.Participante.Club != null ? src.Participante.Club.Sigla : null))
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.Participante != null ? src.Participante.ClubId : null))
                .ForMember(dest => dest.ParticipanteClubId, opt => opt.MapFrom(src => src.Participante != null ? src.Participante.ClubId : null))
                .ForMember(dest => dest.EventoNombre, opt => opt.MapFrom(src => src.EventoPrueba != null && src.EventoPrueba.Evento != null ? src.EventoPrueba.Evento.Nombre : null))
                .ForMember(dest => dest.PruebaNombre, opt => opt.MapFrom(src => 
                    src.EventoPrueba != null && src.EventoPrueba.Prueba != null
                    ? $"{src.EventoPrueba.Prueba.Categoria.Nombre} {src.EventoPrueba.Prueba.Bote.Tipo} {src.EventoPrueba.Prueba.Distancia.Descripcion} {(src.EventoPrueba.Prueba.Sexo != null ? src.EventoPrueba.Prueba.Sexo.Nombre : "")}".Trim()
                    : null));
            
            CreateMap<InscripcionCreateDto, Entidades.Entidades.Inscripcion>();
            CreateMap<InscripcionUpdateDto, Entidades.Entidades.Inscripcion>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeos de Evento
            CreateMap<Entidades.Entidades.Evento, EventoDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Club != null ? src.Club.Nombre : "Federación"))
                .ForMember(dest => dest.HoraInicioEvento, opt => opt.MapFrom(src => src.HoraInicioEvento.ToString(@"hh\:mm")))
                .ForMember(dest => dest.HoraInicioReceso, opt => opt.MapFrom(src => src.HoraInicioReceso.ToString(@"hh\:mm")))
                .ForMember(dest => dest.HoraFinReceso, opt => opt.MapFrom(src => src.HoraFinReceso.ToString(@"hh\:mm")))
                .ForMember(dest => dest.PerfilTiempo, opt => opt.MapFrom(src => src.PerfilTiempo.ToString()));

            CreateMap<EventoCreateDto, Entidades.Entidades.Evento>()
                .ForMember(dest => dest.HoraInicioEvento, opt => opt.MapFrom(src => TimeSpan.Parse(src.HoraInicioEvento)))
                .ForMember(dest => dest.HoraInicioReceso, opt => opt.MapFrom(src => TimeSpan.Parse(src.HoraInicioReceso)))
                .ForMember(dest => dest.HoraFinReceso, opt => opt.MapFrom(src => TimeSpan.Parse(src.HoraFinReceso)))
                .ForMember(dest => dest.PerfilTiempo, opt => opt.MapFrom(src => Enum.Parse<PerfilTiempoEnum>(src.PerfilTiempo)));

            CreateMap<EventoUpdateDto, Entidades.Entidades.Evento>()
                .ForMember(dest => dest.HoraInicioEvento, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.HoraInicioEvento) ? default : TimeSpan.Parse(src.HoraInicioEvento)))
                .ForMember(dest => dest.HoraInicioReceso, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.HoraInicioReceso) ? default : TimeSpan.Parse(src.HoraInicioReceso)))
                .ForMember(dest => dest.HoraFinReceso, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.HoraFinReceso) ? default : TimeSpan.Parse(src.HoraFinReceso)))
                .ForMember(dest => dest.PerfilTiempo, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.PerfilTiempo) ? default : Enum.Parse<PerfilTiempoEnum>(src.PerfilTiempo)))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Mapeos de Fase y Resultados
            CreateMap<Entidades.Entidades.Fase, SportTrack_v1.Controladores.Fase.Dtos.FaseDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.EtapaNombre, opt => opt.MapFrom(src => src.Etapa != null ? src.Etapa.Nombre : string.Empty))
                .ForMember(dest => dest.EtapaOrden, opt => opt.MapFrom(src => src.Etapa != null ? src.Etapa.Orden : 0))
                .ForMember(dest => dest.EventoPruebaId, opt => opt.MapFrom(src => src.Etapa != null ? src.Etapa.EventoPruebaId : 0))
                .ForMember(dest => dest.Prueba, opt => opt.MapFrom(src => src.Etapa != null ? src.Etapa.EventoPrueba : null));
            CreateMap<Entidades.Entidades.Resultado, SportTrack_v1.Controladores.Fase.Dtos.ResultadoFaseDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.ParticipanteId, opt => opt.MapFrom(src => src.Inscripcion != null ? src.Inscripcion.ParticipanteId : null))
                .ForMember(dest => dest.ParticipanteNombre, opt => opt.MapFrom(src => src.Inscripcion != null && src.Inscripcion.Participante != null ? $"{src.Inscripcion.Participante.Nombre} {src.Inscripcion.Participante.Apellido}" : string.Empty))
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Inscripcion != null && src.Inscripcion.Participante != null && src.Inscripcion.Participante.Club != null ? src.Inscripcion.Participante.Club.Nombre : string.Empty))
                .ForMember(dest => dest.ClubSigla, opt => opt.MapFrom(src => src.Inscripcion != null && src.Inscripcion.Participante != null && src.Inscripcion.Participante.Club != null ? src.Inscripcion.Participante.Club.Sigla : string.Empty))
                .ForMember(dest => dest.NumeroCompetidor, opt => opt.MapFrom(src => src.Inscripcion != null ? src.Inscripcion.NumeroCompetidor : string.Empty))
                .ForMember(dest => dest.Tripulantes, opt => opt.MapFrom(src => src.Inscripcion != null ? src.Inscripcion.Tripulantes : null));


            // Mapeos de Usuario
            CreateMap<Usuario, AuthResponseDto>()
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Club != null ? src.Club.Nombre : null));
            CreateMap<RegisterDto, Usuario>();
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Club != null ? src.Club.Nombre : null));

            // Mapeos de Participante
            CreateMap<Entidades.Entidades.Participante, ParticipanteDto>()
                .ForMember(dest => dest.SexoNombre, opt => opt.MapFrom(src => src.Sexo != null ? src.Sexo.Nombre : string.Empty))
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
                .ForMember(dest => dest.ClubNombre, opt => opt.MapFrom(src => src.Club != null ? src.Club.Nombre : string.Empty));
            CreateMap<ParticipanteCreateDto, Entidades.Entidades.Participante>();

            // Mapeos de Prueba y EventoPrueba
            CreateMap<Sexo, SexoDto>();
            CreateMap<Prueba, PruebaDto>()
                .ForMember(dest => dest.SexoNombre, opt => opt.MapFrom(src => src.Sexo != null ? src.Sexo.Nombre : "Mixto"));
            CreateMap<EventoPrueba, EventoPruebaDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()))
                .ForMember(dest => dest.CantidadInscritos, opt => opt.MapFrom(src => src.Inscripciones != null ? src.Inscripciones.Count : 0));
            // Mapeos de SaaS
            CreateMap<PlanSaaS, SportTrack_v1.Controladores.SaaS.Dtos.PlanSaaSDto>();
        }
    }
}
