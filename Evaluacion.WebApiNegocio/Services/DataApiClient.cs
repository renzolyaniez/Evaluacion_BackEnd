namespace Evaluacion.WebApiNegocio.Services
{
    using global::Evaluacion.Models.Dtos;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;

    public class DataApiClient : IDataApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<DataApiClient> _logger;

        public DataApiClient(HttpClient http, ILogger<DataApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<bool> ExistsAlumnoAsync(int alumnoId)
        {
            try
            {
                var res = await _http.GetAsync($"api/Alumno/{alumnoId}");
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comprobando existencia de alumno {AlumnoId}", alumnoId);
                throw;
            }
        }

        public async Task<bool> ExistsCursoAsync(int cursoId)
        {
            try
            {
                var res = await _http.GetAsync($"api/Cursos/{cursoId}");
                return res.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comprobando existencia de curso {CursoId}", cursoId);
                throw;
            }
        }

        public async Task<bool> ExistsInscripcionAsync(int alumnoId, int cursoId)
        {
            try
            {
                var res = await _http.GetAsync($"api/CursoInscripcion/exists?alumnoId={alumnoId}&cursoId={cursoId}");
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("ExistsInscripcionAsync no exitosa: {StatusCode}", res.StatusCode);
                    return false;
                }

                var exists = await res.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comprobando existencia de inscripcion (Alumno {AlumnoId}, Curso {CursoId})", alumnoId, cursoId);
                throw;
            }
        }

        public async Task<CursoInscripcionDto?> CreateInscripcionAsync(CursoInscripcionDto dto)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/CursoInscripcion", dto);

                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("CreateInscripcionAsync no exitosa: {StatusCode}", res.StatusCode);
                    return null;
                }

                var created = await res.Content.ReadFromJsonAsync<CursoInscripcionDto>();
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando inscripcion (Alumno {AlumnoId}, Curso {CursoId})", dto.AlumnoId, dto.CursoId);
                throw;
            }
        }

        public async Task<CursoDto?> CreateCursoAsync(CursoDto dto)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/Cursos", dto);

                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("CreateCursoAsync no exitosa: {StatusCode}", res.StatusCode);
                    return null;
                }

                var created = await res.Content.ReadFromJsonAsync<CursoDto>();
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando curso ({Nombre})", dto?.Nombre);
                throw;
            }
        }

        public async Task<AlumnoDto?> CreateAlumnoAsync(AlumnoDto dto)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/Alumno", dto);

                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("CreateAlumnoAsync no exitosa: {StatusCode}", res.StatusCode);
                    return null;
                }

                var created = await res.Content.ReadFromJsonAsync<AlumnoDto>();
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando alumno ({Email})", dto?.Email);
                throw;
            }
        }

        // Usar exclusivamente el endpoint genérico /api/CursoInscripcion/search?field=CursoId&value={cursoId}
        public async Task<IEnumerable<AlumnoDto>> GetAlumnosByCursoAsync(int cursoId)
        {
            if (cursoId <= 0) return Enumerable.Empty<AlumnoDto>();

            try
            {
                var inscResp = await _http.GetAsync($"api/CursoInscripcion/search?field=CursoId&value={cursoId}");
                if (!inscResp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GetAlumnosByCursoAsync: no se pudo obtener inscripciones (Status {StatusCode})", inscResp.StatusCode);
                    return Enumerable.Empty<AlumnoDto>();
                }

                var inscripciones = await inscResp.Content.ReadFromJsonAsync<IEnumerable<CursoInscripcionDto>>();
                if (inscripciones == null) return Enumerable.Empty<AlumnoDto>();

                var alumnoIds = inscripciones.Select(i => i.AlumnoId).Distinct().ToArray();
                if (alumnoIds.Length == 0) return Enumerable.Empty<AlumnoDto>();

                const int maxParallel = 8;
                using var semaphore = new System.Threading.SemaphoreSlim(maxParallel);

                var tasks = alumnoIds.Select(async id =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var resp = await _http.GetAsync($"api/Alumno/{id}");
                        if (!resp.IsSuccessStatusCode)
                        {
                            _logger.LogWarning("GetAlumnosByCursoAsync: alumno {AlumnoId} no encontrado (Status {Status})", id, resp.StatusCode);
                            return null;
                        }

                        var alumno = await resp.Content.ReadFromJsonAsync<AlumnoDto>();
                        return alumno;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "No se pudo obtener alumno {AlumnoId}", id);
                        return null;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var results = await Task.WhenAll(tasks);
                return results.Where(a => a != null)!.Cast<AlumnoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo alumnos del curso {CursoId}", cursoId);
                throw;
            }
        }
    }
}
