using AutoMapper;
using Evaluacion.Interfaces.Genericos;
using Evaluacion.Models.Dtos;
using Evaluacion.Models.Entidades;
using Microsoft.Extensions.Logging;

namespace Evaluacion.WebApi.Controllers
{
    public class CursosController : BaseEntityDtoController<Curso, CursoDto, CursoDto>
    {
        public CursosController(
            IDatosGenericos<Curso> baseEntitys,
            ILogger<BaseEntityDtoController<Curso, CursoDto, CursoDto>> logger,
            IMapper mapper)
            : base(baseEntitys, logger, mapper)
        {
        }
    }
}
