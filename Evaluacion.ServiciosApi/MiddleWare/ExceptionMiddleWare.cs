using Evaluacion.ServiciosApi.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Evaluacion.ServiciosApi.MiddleWare
{
    public class ExceptionMidleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMidleWare> _logger;

        public ExceptionMidleWare(RequestDelegate next, ILogger<ExceptionMidleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                // Si la respuesta ya comenzó, no podemos modificar las cabeceras / body
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response already started, rethrowing exception so upstream can handle it.");
                    throw;
                }

                // Limpiar cualquier contenido parcial y escribir respuesta JSON consistente
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new CodeErrorException(
                    (int)HttpStatusCode.InternalServerError,
                    ex.Message,
                    ex.StackTrace?.ToString() ?? string.Empty);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
