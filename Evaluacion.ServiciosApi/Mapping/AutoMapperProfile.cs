using AutoMapper;
using Evaluacion.Models.Dtos;
using Evaluacion.Models.Entidades;

namespace Evaluacion.ServiciosApi.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Curso, CursoDto>().ReverseMap();
            CreateMap<Alumno, AlumnoDto>().ReverseMap();
            CreateMap<CursoInscripcion, CursoInscripcionDto>().ReverseMap();


        }
    }
}
