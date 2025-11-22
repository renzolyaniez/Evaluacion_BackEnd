using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.ServiciosApi.Errors
{
    public class CodeErrorResponse
    {
        public CodeErrorResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageStatusCode(statusCode);

        }

        private string GetDefaultMessageStatusCode(int statusCode)
        {

            return statusCode switch
            {

                400 => "El Request enviado tiene errores",
                401 => "permiso denegado para este recurso",
                404 => "The Record Finded not Exist",
                500 => "Se producieron errores en el servidor",
                _ => null


            };
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
