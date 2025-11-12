using MongoDB.Driver;
using DotNetEnv;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using CloudinaryDotNet;



var builder = WebApplication.CreateBuilder(args);


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

MongoClient mongoClient = new MongoClient(linkDb);

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


app.Run();


