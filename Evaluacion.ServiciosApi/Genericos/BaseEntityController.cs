using AutoMapper;
using Evaluacion.Interfaces.Genericos;
using Evaluacion.Models.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq.Expressions;

namespace Evaluacion.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseEntityDtoController<TEntity, TCreateDto, TReadDto> : ControllerBase
        where TEntity : EntidadBase
    {
        protected readonly IDatosGenericos<TEntity> _baseEntitys;
        protected readonly ILogger<BaseEntityDtoController<TEntity, TCreateDto, TReadDto>> _logger;
        protected readonly IMapper _mapper;

        public BaseEntityDtoController(
            IDatosGenericos<TEntity> baseEntitys,
            ILogger<BaseEntityDtoController<TEntity, TCreateDto, TReadDto>> logger,
            IMapper mapper)
        {
            _baseEntitys = baseEntitys;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IReadOnlyList<TReadDto>>> GetAllAsync()
        {
            var (isSuccess, message, datos) = await _baseEntitys.GetAllAsync();
            if (!isSuccess) return BadRequest(message);

            var dtos = _mapper.Map<IReadOnlyList<TReadDto>>(datos);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TReadDto>> GetByIdAsync(int id)
        {
            var (isSuccess, message, dato) = await _baseEntitys.GetAsync(id);
            if (!isSuccess || dato == null) return NotFound(message);

            var dto = _mapper.Map<TReadDto>(dato);
            return Ok(dto);
        }

        [HttpGet("search")]
        public virtual async Task<ActionResult<IReadOnlyList<TReadDto>>> Search([FromQuery] string field, [FromQuery] string value, [FromQuery] int? skip = null, [FromQuery] int? take = null)
        {
            if (string.IsNullOrWhiteSpace(field))
                return BadRequest("El parámetro 'field' es requerido.");

            var memberInfo = typeof(TEntity).GetProperty(field) ?? (object)typeof(TEntity).GetField(field);
            if (memberInfo == null)
                return BadRequest($"La propiedad '{field}' no existe en el tipo {typeof(TEntity).Name}.");

            Type memberType;
            if (memberInfo is System.Reflection.PropertyInfo pi) memberType = pi.PropertyType;
            else if (memberInfo is System.Reflection.FieldInfo fi) memberType = fi.FieldType;
            else return BadRequest("No se pudo resolver la propiedad solicitada.");

            if (!TryConvert(value, memberType, out object convertedValue))
                return BadRequest($"No se pudo convertir el valor '{value}' al tipo {memberType.Name}.");

            // Construir expresión: e => e.{field} == convertedValue
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var member = Expression.PropertyOrField(parameter, field);
            var constant = Expression.Constant(convertedValue, member.Type);
            Expression body = Expression.Equal(member, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            var (isSuccess, message, datos) = await _baseEntitys.SearchWithFilter(lambda, skip, take);
            if (!isSuccess) return BadRequest(message);

            var dtos = _mapper.Map<IReadOnlyList<TReadDto>>(datos);
            return Ok(dtos);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddItemAsync([FromBody] TCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(createDto);
            var (isSuccess, message, datoId) = await _baseEntitys.AddItemAsync(entity);

            if (!isSuccess) return BadRequest(message);

            var readDto = _mapper.Map<TReadDto>(datoId);

            // Construir la URI explícitamente para evitar problemas de resolución de rutas
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;
            var location = $"/api/{controllerName}/{datoId.Id}";

            return Created(location, readDto);
        }

        [HttpPut]
        public virtual async Task<IActionResult> UpdateItemAsync([FromBody] TReadDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<TEntity>(updateDto);
            var (isSuccess, message) = await _baseEntitys.UpdateItemAsync(entity);

            if (!isSuccess) return BadRequest(message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> DeleteItemAsync(int id)
        {
            var (isSuccess, message) = await _baseEntitys.DeleteItemAsync(id);
            if (!isSuccess) return BadRequest(message);
            return NoContent();
        }

        // Helper para conversión de string a tipo destino (soporta nullable y tipos comunes)
        private bool TryConvert(string stringValue, Type targetType, out object result)
        {
            result = null;
            if (targetType == typeof(string))
            {
                result = stringValue;
                return true;
            }

            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlying.IsEnum)
                {
                    result = Enum.Parse(underlying, stringValue, ignoreCase: true);
                    if (Nullable.GetUnderlyingType(targetType) != null)
                    {
                        // mantener tipo nullable
                        var nullableType = typeof(Nullable<>).MakeGenericType(underlying);
                        result = Convert.ChangeType(result, underlying, CultureInfo.InvariantCulture);
                    }
                    return true;
                }

                if (underlying == typeof(Guid))
                {
                    result = Guid.Parse(stringValue);
                    return true;
                }

                if (underlying == typeof(DateTime))
                {
                    result = DateTime.Parse(stringValue, CultureInfo.InvariantCulture);
                    return true;
                }

                if (underlying == typeof(bool))
                {
                    result = bool.Parse(stringValue);
                    return true;
                }

                // numéricos y otros
                result = Convert.ChangeType(stringValue, underlying, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
