using System.Threading.Tasks;
using Evaluacion.Models.Dtos;

namespace Evaluacion.WebApiNegocio.Services
{
    public interface IDataApiClient
    {
        Task<bool> ExistsAlumnoAsync(int alumnoId);
        Task<bool> ExistsCursoAsync(int cursoId);
        Task<bool> ExistsInscripcionAsync(int alumnoId, int cursoId);
        Task<CursoInscripcionDto?> CreateInscripcionAsync(CursoInscripcionDto dto);
        Task<CursoDto?> CreateCursoAsync(CursoDto dto);
        Task<AlumnoDto?> CreateAlumnoAsync(AlumnoDto dto);
        Task<IEnumerable<AlumnoDto>> GetAlumnosByCursoAsync(int cursoId);
    }
}
