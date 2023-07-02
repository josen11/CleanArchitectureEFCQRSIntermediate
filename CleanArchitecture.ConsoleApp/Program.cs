using CleanArchitecture.Data;
using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static System.Net.WebRequestMethods;

// Instanciamos un objeto con el new() que es una nueva manera que viene en .Net 6
StreamerDbContext dbContext = new();
// Call methods
// await AddNewRecords("disney","http://wwww.disney.com");
// QueryStreaming();
// await QueryFilter("Net");
// await QueryMethods();
// await QueryLinq();
// await TrackingAndNotTracking();
// await QueryLinqFilter("Net");
// await QueryLinqFilter("Amazon");
// await AddNewStreamerWithVideo();
await GetMultipleEntities();

// Notes
// - Para manejar la advertencia de posibble Null reference Argument es que agregar el ! de tal manera que el compilador entiendo que este valor ya esta instanciado
// - Entitify Framework podemos trabajar utilizando funciones agregadas y expresiones Lambda o utilizando LinQ
// - No podemos utilizar AsNoTracking() cuando utilicemos el metodo FindAsync


// Get Multiple Entities
async Task GetMultipleEntities()
{
    // Include ayuda a traer las entidades relacionadas en una relacion 1..M. Basicamente es como un inner join que devuelve todas las columnas de ambas entidades
    // var videoWithActors = await dbContext!.Videos!.Include(q => q.Actores).FirstOrDefaultAsync(q => q.Id == 1);

    // Tambien podemos hacer que devuelva algunas consultas
    // var actor = await dbContext!.Actores!.Select(q => q.Nombre).ToListAsync();

    // Tambien podemos utlizar el Select y el include
    // Por defecto el Select solo permite devolver 1 columna mediante la expresion Lambda, para devolver multiples columnas necesitas hacer una proyeccion (Alias).
    // Se recomienda aplicar el WHERE A nivel de la entidad principal
    var videoWithDirector = await dbContext!.Videos!
                                .Where(q => q.Director != null)
                                .Include(q => q.Director)
                                .Select( q =>
                                    new {
                                        Director_Nombre_Completo = $"{q.Director.Nombre} {q.Director.Apellido}",
                                        Movie = q.Nombre
                                    }
                                )
                                .ToListAsync();
    // Los alias establecidos en la proyeccion se utilizan
    foreach (var movie in videoWithDirector)
    {
        Console.WriteLine($"Movie: {movie.Movie} - Director: {movie.Director_Nombre_Completo}");
    }
}

// Create with 1..1
async Task AddNewDirectorWithVideo()
{
    var director = new Director
    {
        Nombre = "Lorenzo",
        Apellido = "Bassteri",
        VideoId = 1
    };
    await dbContext.AddAsync(director);
    await dbContext.SaveChangesAsync();
}


// Create with M..M
async Task AddNewActorWithVideo()
{
    var actor = new Actor
    {
        Nombre = "Brad",
        Apellido = "Pitt"
    };
    await dbContext.AddAsync(actor);
    await dbContext.SaveChangesAsync();

    var videoActor = new VideoActor
    {
        ActorId = actor.Id,
        VideoId = 1
    };
    await dbContext.AddAsync(videoActor);
    await dbContext.SaveChangesAsync();
}

// Create with 1..M
async Task AddNewStreamerWithVideoId()
{
    var hungerGames = new Video
    {
        Nombre = "Hunger Games",
        StreamerId = 2
    };
    await dbContext.AddAsync(hungerGames);
    await dbContext.SaveChangesAsync();
}

async Task AddNewStreamerWithVideo()
{
    // Genial una manera mas sencilla de registrar records con relaciones utilizando la referencia al objeto que representa la Entidad Foranea 
    var pantaya = new Streamer
    {
        Nombre = "Pantaya"
    };

    var hungerGames = new Video
    {
        Nombre = "Hunger Games",
        Streamer = pantaya
    };
    await dbContext.AddAsync(hungerGames);
    await dbContext.SaveChangesAsync();
}

async Task TrackingAndNotTracking()
{ 
    var streamerWithTracking = await dbContext!.Streamers!.FirstOrDefaultAsync(x => x.Id==1);
    var streamerWithNoTracking = await dbContext!.Streamers!.AsNoTracking().FirstOrDefaultAsync(x => x.Id == 2);

    streamerWithTracking!.Nombre = "Netflix updated";
    // Con el AsNoTracking no permite guardar cambios entonces basicamente libera el objeto de la memoria temporal, entonces por esa razon no se puede guardar.
    // Un buen caso de uso del AsNoTracking es cuando utilicemos consultas a la BD que tienen gran demanda pero que no deribaran en una edicion o consulta extra sobre el resultado.
    streamerWithNoTracking!.Nombre = "Amazon Prime updated";

    await dbContext!.SaveChangesAsync();
}

async Task QueryLinqFilter(string filter)
{
    // i representa a la entidad que se esta consultando, por eso le damos select i y por eso en from i in (dbContext.Entidad) asignamos a al entidad a i. Where antes del Select. El select siempre va al final.
    var streamers = await (from i in dbContext.Streamers
                           where EF.Functions.Like(i.Nombre!, $"%{filter}%")
                           select i).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

async Task QueryLinq()
{
    // i representa el resultado general, por eso le damos select i y por eso es from i in (dbContext.Entidad)
    var streamers = await (from i in dbContext.Streamers
                     select i).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

async Task QueryMethods()
{
    // Functions: First, FirstOrDefault, Single and SingleOrDefault
    // Ya que vamos a utilizar multiples veces el dbContext!.Streamers! mejor lo asignamos a una variable
    var streamer = dbContext!.Streamers!;
    // Return the first one. Pero si no se encontrase ningun registro generaria una excepcion y pararia el programa. Ahora este metodo lo que hace es recoger la coleccion que hace match con la condicion y devolver el primero
    var firstAsync = await streamer.Where(y => y.Nombre!.Contains("a")).FirstAsync();
    // Para evitar que el codigo previo genere una excepcion cuando no se encuentra ningun registro. En este caso si no encuentra un valor devolvera un null.
    var firstOrDefaultAsync = await streamer.Where(y => y.Nombre!.Contains("a")).FirstOrDefaultAsync();
    // Asimismo podemos definir un FirstOrDefaultAsync() de otra manera
    var firstOrDefaultAsync2 = await streamer.FirstOrDefaultAsync(y => y.Nombre!.Contains("a"));
    // Single es un poco diferente por que lo que hace este es revisar y devolver un valor valido siempre y cuando exista unicamente 1 record que haga match con la condicion, es decir si no existe ningun o mas de 1 genera una excepcion.
    var syngleAsync = await streamer.Where(y => y.Id==1).SingleAsync();
    // Respeta las mismas reglas del single pero si no encuntra ninguna valor arroja null como valor por defecto.
    var syngleOrDefaultAsync = await streamer.Where(y => y.Id == 1).SingleOrDefaultAsync();

    // Buscar un record por su Id
    var result = await streamer.FindAsync(1);

}

async Task QueryFilter(string filter)
{
    // Utilizamos la funcion agregada WHERE y dentro de esta una expresion Lambda
    // Para que pueda ejecutar la funcion agregada WHERE es necesario poner el ToListAsync()
    // var streamers = await dbContext!.Streamers!.Where(x => x.Nombre!.Contains(filter)).ToListAsync();
    // Ademas de las expresiones Lambda podremos utilizar las funciones de EF. Eg. Like
    var streamers = await dbContext!.Streamers!.Where(x => EF.Functions.Like(x.Nombre!,$"%{filter}%")).ToListAsync();

    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

void QueryStreaming()
{
    var streamers = dbContext!.Streamers!.ToList(); // Get all
    foreach (var streamer in streamers)
    {
        Console.WriteLine($"{streamer.Id} - {streamer.Nombre}");
    }
}

// Factorizar codigo = Realizar encapsulamiento para que se genere un metodo
async Task AddNewRecords(string streamerName, string streamerUrl)
{
    Streamer streamer = new()
    {
        Nombre = streamerName,
        Url = streamerUrl
    };
    // El signo de ! es para indicar que el objeto esta instanciado es decir que ya existe.
    dbContext!.Streamers!.Add(streamer);
    // Para confirmar la operacion. Por ser un proceso asincrono debemos usar la palabra await
    await dbContext.SaveChangesAsync();
    // Lo genial es que tambien al ejecutar la transaccion le asigna el Id de BD a la Entidad, el cual podemos utilizar despues de esta linea de codigo

    // Podemos agregar varios records a la vez
    var movies = new List<Video>() {
    new Video() { Nombre="Mad Max", StreamerId=streamer.Id},
    new Video() { Nombre="Fast and furious", StreamerId=streamer.Id},
    new Video() { Nombre="Avengers Last Game", StreamerId=streamer.Id}
    };
    // Para ello utilizamos AddRange
    await dbContext.AddRangeAsync(movies);
    // Confirmamos operacion
    await dbContext.SaveChangesAsync();
}