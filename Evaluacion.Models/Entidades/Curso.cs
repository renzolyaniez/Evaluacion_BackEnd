using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.Models.Entidades
{
    public class Curso: EntidadBase
    {
       
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Profesor { get; set; }  

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        // Relación: Un curso puede tener muchos inscriptos
        public ICollection<CursoInscripcion> CursosInscripciones { get; set; }
    }
}
