using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using VillaAPI;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository;
using VillaAPI.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnetion"));
    });

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
#endregion

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
#region AutoMapper

builder.Services.AddAutoMapper(typeof(MappingConfig));

#endregion

builder.Services.AddResponseCaching();

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();

string key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

#region Serilog

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug().WriteTo
    .File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();

#endregion

builder.Services.AddControllers
    (options =>
    {
        options.CacheProfiles.Add("Default30",
        new CacheProfile()
        {
            Duration = 30
        });
        //options.ReturnHttpNotAcceptable = true;} // For Postman

    })
    .AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region ApiVersion

builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

#endregion

builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Villa V1",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "AlexSHZ",
            Url = new Uri("https://www.linkedin.com/in/%D0%B0%D0%BB%D0%B5%D0%BA%D1%81%D0%B0%D0%BD%D0%B4%D1%80-%D1%88%D1%82%D0%B5%D1%84%D0%B0%D0%BD-809258245/")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Villa V2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "AlexSHZ",
            Url = new Uri("https://www.linkedin.com/in/%D0%B0%D0%BB%D0%B5%D0%BA%D1%81%D0%B0%D0%BD%D0%B4%D1%80-%D1%88%D1%82%D0%B5%D1%84%D0%B0%D0%BD-809258245/")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "VillaV1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "VillaV2");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

