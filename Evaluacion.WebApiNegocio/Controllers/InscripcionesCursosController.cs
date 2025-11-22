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
    public class InscripcionesCursosController : ControllerBase
    {
        private readonly IDataApiClient _dataApi;
        private readonly ILogger<InscripcionesCursosController> _logger;

        public InscripcionesCursosController(IDataApiClient dataApi, ILogger<InscripcionesCursosController> logger)
        {
            _dataApi = dataApi;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CrearInscripcion([FromBody] CursoInscripcionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar existencia de Alumno
            var alumnoExiste = await _dataApi.ExistsAlumnoAsync(dto.AlumnoId);
            if (!alumnoExiste)
                return NotFound(new { message = $"Alumno con id {dto.AlumnoId} no encontrado." });

            // Validar existencia de Curso
            var cursoExiste = await _dataApi.ExistsCursoAsync(dto.CursoId);
            if (!cursoExiste)
                return NotFound(new { message = $"Curso con id {dto.CursoId} no encontrado." });

            // Validar duplicado de inscripción
            var yaInscripto = await _dataApi.ExistsInscripcionAsync(dto.AlumnoId, dto.CursoId);
            if (yaInscripto)
                return Conflict(new { message = $"Alumno {dto.AlumnoId} ya está inscripto en el curso {dto.CursoId}." });

            // Llamar API de datos para crear la inscripción
            var creado = await _dataApi.CreateInscripcionAsync(dto);
            if (creado == null)
                return StatusCode(502, new { message = "Error al crear la inscripción en la API de datos." });

            // Devuelve 201 con el recurso creado (ruta construida hacia la API de datos)
            var location = $"/api/CursoInscripcion/{creado.Id}";
            return Created(location, creado);
        }
    }
}
