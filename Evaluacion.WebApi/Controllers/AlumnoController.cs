using AutoMapper;
using Evaluacion.Interfaces.Genericos;
using Evaluacion.Models.Dtos;
using Evaluacion.Models.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion.WebApi.Controllers
{
    public class AlumnoController : BaseEntityDtoController<Alumno, AlumnoDto, AlumnoDto>
    {
        public AlumnoController(IDatosGenericos<Alumno> baseEntitys, ILogger<BaseEntityDtoController<Alumno, AlumnoDto, AlumnoDto>> logger, IMapper mapper) : base(baseEntitys, logger, mapper)
        {
        }
    }
}
