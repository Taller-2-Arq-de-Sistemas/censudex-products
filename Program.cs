using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using CloudinaryDotNet;
using censudex_products.src.Services;
using censudex_products.src.Seeders;
using censudex_products.src.Interfaces;
using censudex_products.src.Repositories;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();





Env.Load();

builder.Services
.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


// Configuracion MongoAtlas
string linkDb = Environment.GetEnvironmentVariable("DATABASE_URL") ?? "mongodb+srv://<UserName>:<Contraseña>@base-ticket-service.y9rcn0b.mongodb.net/?retryWrites=true&w=majority&appName=Base-Ticket-Service";
string nameDb = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "Data-base-ticket-service";

var settings = MongoClientSettings.FromConnectionString(linkDb);
settings.SslSettings = new SslSettings
{
    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
};
settings.UseTls = true;
settings.AllowInsecureTls = true; // Add this for development

var mongoClient = new MongoClient(settings);




var database = mongoClient.GetDatabase(nameDb);



// Configuracion Cloudinary
var account = new Account(
    Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
    Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
);
var cloudinary = new Cloudinary(account);



builder.Services.AddSingleton(cloudinary);
builder.Services.AddSingleton(database);
builder.Services.AddControllers();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGrpcService<ProductGrpcService>();



// Ejecutar seeder

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    var productSeeder = new ProductSeeder(db);
    await productSeeder.SeedAsync();
}

app.Run();


