using Evaluacion.WebApiNegocio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ListaAlumnosCursoController : ControllerBase
{
    private readonly IDataApiClient _dataApi;
    private readonly ILogger<ListaAlumnosCursoController> _logger;

    public ListaAlumnosCursoController(IDataApiClient dataApi, ILogger<ListaAlumnosCursoController> logger)
    {
        _dataApi = dataApi;
        _logger = logger;
    }

    // GET api/ListaAlumnosCurso/{cursoId}
    [HttpGet("{cursoId}")]
    public async Task<IActionResult> GetAlumnosByCurso(int cursoId)
    {
        if (cursoId <= 0) return BadRequest(new { message = "cursoId debe ser mayor a 0." });

        var cursoExiste = await _dataApi.ExistsCursoAsync(cursoId);
        if (!cursoExiste) return NotFound(new { message = $"Curso con id {cursoId} no encontrado." });

        var alumnos = await _dataApi.GetAlumnosByCursoAsync(cursoId);
        return Ok(alumnos);
    }
}