using AutoMapper;
using Evaluacion.Interfaces.Genericos;
using Evaluacion.Models.Dtos;
using Evaluacion.Models.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion.WebApi.Controllers
{
    public class CursoInscripcionController : BaseEntityDtoController<CursoInscripcion, CursoInscripcionDto, CursoInscripcionDto>
    {
        public CursoInscripcionController(IDatosGenericos<CursoInscripcion> baseEntitys, ILogger<BaseEntityDtoController<CursoInscripcion, CursoInscripcionDto, CursoInscripcionDto>> logger, IMapper mapper) : base(baseEntitys, logger, mapper)
        {
        }
    }
}
