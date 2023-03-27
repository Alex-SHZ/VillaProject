using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using VillaAPI;
using VillaAPI.Data;
using VillaAPI.Repository;
using VillaAPI.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnetion"));
    });

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty

#region AutoMapper

builder.Services.AddAutoMapper(typeof(MappingConfig));

#endregion

builder.Services.AddScoped<IVillaRepository, VillaRepository>();

#region Serilog

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug().WriteTo
    .File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();

#endregion

builder.Services.AddControllers
    (//options => { options.ReturnHttpNotAcceptable = true;} // For Postman
     )
    .AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();

