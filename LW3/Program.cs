using FluentValidation;
using FluentValidation.AspNetCore;

using LW3.Settings;
using LW3.Mapping;
using LW3.Middleware;
using LW3.Repositories;
using LW3.Services;
using LW3.Validators;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AutoMapper;
using System.Text.Json.Serialization;
using System.Security.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<AdminSeedSettings>(builder.Configuration.GetSection(AdminSeedSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("У конфігурації відсутня секція Jwt.");
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
    throw new InvalidOperationException("Jwt:SecretKey має містити щонайменше 32 символи.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero   // токен перестає діяти рівно в момент закінчення строку
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<AdminSeedService>();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));
//builder.Services.AddSingleton<IMongoClient>(sp =>
//{
//    var s = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    var settings = MongoClientSettings.FromConnectionString(s.ConnectionString);
//    settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
//    return new MongoClient(settings);
//});

//builder.Services.Configure<MongoDbSettings>(
//    builder.Configuration.GetSection(MongoDbSettings.SectionName));

//builder.Services.AddSingleton<IMongoClient>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    return new MongoClient(settings.ConnectionString);
//});

//builder.Services.AddSingleton<IMongoDatabase>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
//    return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
//});
// Налаштування MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));

// Реєстрація MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    var client = new MongoClient(settings.ConnectionString);

    return client;
});

// Реєстрація бази даних
//var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(Int32.Parse(port));
});
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    var client = sp.GetRequiredService<IMongoClient>();

    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IMechanicRepository, MechanicRepository>();
builder.Services.AddScoped<IRepairOrderRepository, RepairOrderRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IPartService, PartService>();


builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IMechanicService, MechanicService>();
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleDtoValidator>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Вставте access token (без слова Bearer). Кнопка Authorize вгорі сторінки."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});


// MongoDB — Singleton


// Репозиторії — Scoped
builder.Services.AddScoped<IRepairOrderRepository, RepairOrderRepository>();

// Сервіси — Scoped
builder.Services.AddScoped<IRepairOrderService, RepairOrderService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<PersistenceMappingProfile>();
    cfg.AddProfile<ApiMappingProfile>();
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
