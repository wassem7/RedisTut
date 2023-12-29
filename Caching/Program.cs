using System.Text.Json.Serialization;
using Caching.Data;
using Caching.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// builder
//     .Services
//     .AddControllers()
//     .AddNewtonsoftJson(options =>
//     {
//         options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
//         options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//     });
builder.Services.AddScoped<ICacheService, CacheService>();
builder
    .Services
    .AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(
        opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString"))
    );
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
