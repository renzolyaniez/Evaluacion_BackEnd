using Evaluacion.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.Models.Dtos
{
    public class CursoInscripcionDto:EntidadBase
    {
        [Required]
        public int AlumnoId { get; set; }

        [Required]
        public int CursoId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}
