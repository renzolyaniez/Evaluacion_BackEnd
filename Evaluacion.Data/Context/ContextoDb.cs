using Evaluacion.Models.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
namespace Evaluacion.Data.Context
{
    public class ContextoDb : DbContext

    {
        public ContextoDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<CursoInscripcion> CursosInscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Alumno 1 ----> N Inscripciones
            modelBuilder.Entity<CursoInscripcion>()
                .HasOne(ci => ci.Alumno)
                .WithMany(a => a.CursosInscripciones)
                .HasForeignKey(ci => ci.AlumnoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Curso 1 ----> N Inscripciones
            modelBuilder.Entity<CursoInscripcion>()
                .HasOne(ci => ci.Curso)
                .WithMany(c => c.CursosInscripciones)
                .HasForeignKey(ci => ci.CursoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
