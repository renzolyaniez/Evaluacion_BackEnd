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
    public class AgregarAlumnoController : ControllerBase
    {
        private readonly IDataApiClient _dataApi;
        private readonly ILogger<AgregarAlumnoController> _logger;

        public AgregarAlumnoController(IDataApiClient dataApi, ILogger<AgregarAlumnoController> logger)
        {
            _dataApi = dataApi;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CrearAlumno([FromBody] AlumnoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Regla simple local: el email debe tener formato (DataAnnotations ya lo valida) y no ser vacío.
            var creado = await _dataApi.CreateAlumnoAsync(dto);
            if (creado == null)
                return StatusCode(502, new { message = "Error al crear el alumno en la API de datos." });

            var location = $"/api/Alumnos/{creado.Id}";
            return Created(location, creado);
        }
    }
}
