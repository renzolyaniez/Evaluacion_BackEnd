using System.Threading.Tasks;
using Evaluacion.Models.Dtos;
using Evaluacion.WebApiNegocio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Evaluacion.WebApiNegocio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgregarCursoController : ControllerBase
    {
        private readonly IDataApiClient _dataApi;
        private readonly ILogger<AgregarCursoController> _logger;

        public AgregarCursoController(IDataApiClient dataApi, ILogger<AgregarCursoController> logger)
        {
            _dataApi = dataApi;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CrearCurso([FromBody] CursoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Opcional: validar reglas de negocio locales antes de llamar al Data API (fechas, etc.)
            if (dto.FechaFin < dto.FechaInicio)
                return BadRequest(new { message = "FechaFin debe ser mayor o igual a FechaInicio." });

            var creado = await _dataApi.CreateCursoAsync(dto);
            if (creado == null)
                return StatusCode(502, new { message = "Error al crear el curso en la API de datos." });

            var location = $"/api/Cursos/{creado.Id}";
            return Created(location, creado);
        }
    }
}
