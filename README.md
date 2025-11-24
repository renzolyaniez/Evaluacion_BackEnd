## Evaluacion Back End .net
## Tecnología .Net 8, Base de Datos Sql Server.

Descarga y puesta en Marcha
Descargar el proyecto: 
  Clonar el proyecto en el disco local 
  ## Configurar las variables en appsettings para la conexion con la base de datos.
  abrir AppSettings.json del proyecto Evaluacion.WebApi y ajustar el connectionstring de acuerdo a la base que se quiera utilizar
```
"ConnectionStrings": {
  "DefaultConnection": "Server=nombre Servidor SQL;Database=EvaluacionBackEnd;Trusted_Connection=True;TrustServerCertificate=True;"
},

```
  una vez configurada la cadena de conexion, cargar la consola de administracion de paquetes nuget, seleccionar el proyecto Evaluacion.Data
  Tener como proyecto de inicio Evaluacion.WebApi
  Ejecutar desde la consola: 
  update-database
  esto genera la base de datos y las tablas necesarias para probar la solución.

  ## Configurar secret key para uso jwt (opcional) el proyecto funciona correctamente con los valores actuales, pero pueden cambiarse
  abrir el archivo appsettings.json del proyecto Evaluacion.WebApi 
  y si es necesario cambiar la secret key, si se cambia habra que cambiar en la clase program el token generado en la clase program.cs de ambos proyectos
  para que funcione la seguridad en swagger para poder probar todos los endpoint

## Pruebas con Swagger
En la clase Program.cs de ambos proyectos web api, tenemos definido el servicio de swagger para las pruebas en desarrollo, esto habilitara un boton
Authorize en la interfaz de la aplicacion web api, donde nos permitira loguear u token. 
en la linea  Description = "Baerer "......    hay puesto un token valido con 1 año de validez, en la interfaz de swagger con solo copiar y pegar 
el token que muestra dara autorizacion a todos los endpoint de manera facil para poder probar.
```
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Evaluacion API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyIiwibmJmIjoxNzYzODEzMzI4LCJleHAiOjE3OTUzNDkzMjgsImlzcyI6ImN1c3RvbSJ9.cSzKlWuvLs7JKmXMmppI6RULcoACQXQxsxvk9Ov18lw",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});
```
## Estructura de la Solucion

La  Solucion esta compuesta de estos pryectos: 

- **Evaluacion._GeneraTokenParaPruebas** (Aplicacion de consola, solo genera tokens para prueba de ser necesario utilizar devuelve con la secret key que esta harcodeada en el codigo que es la misma que esta en los appsettings un token valido para poder probar, de todas formas el token definido en el builder de ambos proyectos web api el token puesto tiene una validez de 1 año)
  
- **Evaluacion.Data** (Biblioteca de clases que contiene la definicion del ContextDb y aqui es donde se alojan las migraciones de entity framework para poder generar la base de datos.)

- **Evaluacion.Interfaces** ( Biblioteca de clases, donde estan declarados los contratos de las implementaciones comunes a uno o mas proyectos)

- **Evaluacion.Repository** ( Biblioteca de clases, en la cual estan las implementaciones de las interfaces definidas en Interfaces.)

- **Evaluacion.Models** (Biblioteca de clases, en la que estan definidas las Entidades que representan los datos en la base de datos y los DTOs que se utilizaran para interactuar con los distintos Endpoint de la solucion)

- **Evaluacion.ServiciosApi** (Biblioteca de clases en el cual estan definidos, clases y metodos comunes compartidos para todas las aplicaciones api web de la solucion.
  En esta biblioteca tenemos definido el Middleware para el control general de errores, El controlador Generico (BaseEntityController) que utiliza la implementacion de IDatosGenericos para que por herencia, se      pueda generar un CRUD completo de APi con algunas pocas lineas de codigo, y la implementacion de AutoMapper para mover datos de los DTO al Modelo que impactara en la base de datos. (mas adelante describo un    caso de uso).

- **Evaluacion.WebApi** (aplicacion que expone las api de manipulacion de los datos de las 3 entidades definidas: Cursos, Alumnos, CursosInscripcion)
  
- **Evaluacion.WebApiNegocio** (aplicacion que expone la capa superior de Apis para 4 procesos basicos de Alta de Alumnos, Alta de Cursos, Inscripciones y Listados, esta capa, contiene la logica del negocio sin tener acceso directo a los datos de la base, consumiendo los Endpoint declarados en Evaluacion.WebApi).


## Algunas Cosas para Destacar

El proyecto **Evaluacion.WebApi**, esta implementado con clases Genericas, tanto en la implementacion del patron repository como en los controladores, y el uso de AutoMapper para pasar los datos del DTO a las Entidades, de manera tal que agregar un nuevo Crud y desplegar las Api generales requiere de estos 3 simples pasos: 

1) Crear la Entidad, agregar el Dto y agregar la entidad al ConextDb
2) Agregar el Mapeo entre la Entidad u el Dto en la clase AutoMapperProfile
3) Agregar el Controlador que herede de BaseEntityDtoController

Ejemplo si agrego una nueva entidad llamada ProgramasEstudio, para darle la funcionalidad deberia hacer esto: 

En
Evaluacion.Data.Context clase ContextoDb.cs
 agregar el DbSet         
 ```
 public DbSet<ProgramasEstudio> ProgramasEstudio { get; set; }
 ```
En
Evaluacion.ServiciosApi.Mapping clase AutoMapperProfile.cs
agregar el AutoMap
```
CreateMap<ProgramasEstudio, ProgramasEstudioDto>().ReverseMap();
```

y por ultimo Agregar el Controlador en en Evaluacion.WebApi.Controllers 

ProgramasEstudioController.cs
  ```
  using AutoMapper;
  using Evaluacion.Interfaces.Genericos;
  using Evaluacion.Models.Dtos;
  using Evaluacion.Models.Entidades;
  using Microsoft.AspNetCore.Mvc;
  
  namespace Evaluacion.WebApi.Controllers
  {
      public class ProgramasEstudioController : BaseEntityDtoController<ProgramasEstudio, ProgramasEstudioDto, ProgramasEstudioDto>
      {
          public AlumnoController(IDatosGenericos<ProgramasEstudio> baseEntitys, ILogger<BaseEntityDtoController<ProgramasEstudio, ProgramasEstudioDto, ProgramasEstudioDto>> logger, IMapper mapper) : base(baseEntitys, logger, mapper)
          {
          }
      }
  }
```
y solo con estas pocas lineas queda funcionando el nuevo Crud de cualquier entidad, no olvidar si se esta usando CodeFirst, crear una nueva migracion, y actualizar la base.


En la capa superior de Api Evaluacion.WebApiNegocio, algunos puntos para mencionar, serian, 

implemantacion de la clase BearerTokenHandler para trasladar el token recibido en la capa de negocio hacia la capa de datos, tambien se podria mantener distintas secret key para separar la seguridad entre ambas capas. 

Se podria mejorar la consulta para que sea mas optima en cuanto a rendimiento, pero para no alterar la busqueda generica en la api de datos, quedo de esta forma, una manera posible podria ser crear una vista en la base de datos que resuelva las relaciones y recupere solo los campos utilizados.
