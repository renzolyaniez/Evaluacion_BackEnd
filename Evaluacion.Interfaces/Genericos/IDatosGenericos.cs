using Evaluacion.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Evaluacion.Interfaces.Genericos
{
    public interface IDatosGenericos<T> where T : EntidadBase
    {
        Task<(bool isSuccess, string Message, IEnumerable<T> Datos)> GetAllAsync();
        Task<(bool isSuccess, string Message, T DatoId)> AddItemAsync(T item);
        Task<(bool isSuccess, string Message)> UpdateItemAsync(T item);
        Task<(bool isSuccess, string Message)> DeleteItemAsync(int Id);
        Task<(bool isSuccess, string Message, T DatoId)> GetAsync(int Id);
        Task<(bool isSuccess, string Message, IEnumerable<T> Datos)> SearchWithFilter(
           Expression<Func<T, bool>> filter,
           int? skip = null,
           int? take = null
       );

    }
}
