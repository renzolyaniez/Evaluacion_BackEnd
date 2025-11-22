using Evaluacion.Data.Context;
using Evaluacion.Interfaces.Genericos;
using Evaluacion.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Evaluacion.Repository.Genericos
{
    public class DatosGenericos<T> : IDatosGenericos<T> where T : EntidadBase
    {
        private readonly ContextoDb _db;

        public DatosGenericos(ContextoDb db)
        {
            _db = db;
        }

        public async Task<(bool isSuccess, string Message, IEnumerable<T> Datos)> GetAllAsync()
        {
            try
            {
                var datos = await _db.Set<T>()
                                     .AsNoTracking()
                                     .ToListAsync()
                                     .ConfigureAwait(false);

                return (true, "Datos obtenidos correctamente.", datos);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener datos: {ex.Message}", Array.Empty<T>());
            }
        }

        public async Task<(bool isSuccess, string Message, T DatoId)> AddItemAsync(T item)
        {
            if (item == null)
                return (false, "El elemento proporcionado es nulo.", null!);

            try
            {
                item.Id = 0;
                _db.Set<T>().Add(item);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return (true, "Elemento agregado correctamente.", item);
            }
            catch (Exception ex)
            {
                return (false, $"Error al agregar elemento: {ex.Message}", null!);
            }
        }

        public async Task<(bool isSuccess, string Message)> UpdateItemAsync(T item)
        {
            if (item == null)
                return (false, "El elemento proporcionado es nulo.");

            try
            {
                var dbSet = _db.Set<T>();
                var existing = await dbSet.FindAsync(item.Id).ConfigureAwait(false);

                if (existing == null)
                    return (false, "Elemento no encontrado.");

                _db.Entry(existing).CurrentValues.SetValues(item);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return (true, "Elemento actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar elemento: {ex.Message}");
            }
        }

        public async Task<(bool isSuccess, string Message)> DeleteItemAsync(int Id)
        {
            try
            {
                var dbSet = _db.Set<T>();
                var existing = await dbSet.FindAsync(Id).ConfigureAwait(false);

                if (existing == null)
                    return (false, "Elemento no encontrado.");

                dbSet.Remove(existing);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return (true, "Elemento eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar elemento: {ex.Message}");
            }
        }

        public async Task<(bool isSuccess, string Message, IEnumerable<T> Datos)> SearchWithFilter(
            Expression<Func<T, bool>> filter,
            int? skip = null,
            int? take = null
        )
        {
            if (filter == null)
                return (false, "El filtro proporcionado es nulo.", Array.Empty<T>());

            try
            {
                IQueryable<T> query = _db.Set<T>().Where(filter).AsNoTracking();

                if (skip.HasValue && skip.Value > 0)
                    query = query.Skip(skip.Value);

                if (take.HasValue && take.Value > 0)
                    query = query.Take(take.Value);

                var resultados = await query.ToListAsync().ConfigureAwait(false);
                return (true, "Búsqueda completada correctamente.", resultados);
            }
            catch (Exception ex)
            {
                return (false, $"Error durante la búsqueda: {ex.Message}", Array.Empty<T>());
            }
        }

        public async Task<(bool isSuccess, string Message, T DatoId)> GetAsync(int Id)
        {
            try
            {
                if (Id <= 0)
                    return (false, "Id no válido.", null!);

                var dato = await _db.Set<T>()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => e.Id == Id)
                                    .ConfigureAwait(false);

                if (dato == null)
                    return (false, "Elemento no encontrado.", null!);

                return (true, "Elemento obtenido correctamente.", dato);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener elemento: {ex.Message}", null!);
            }
        }
    }
}
