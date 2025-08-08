using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;
using Venice.Orders.API.Token;
using Venice.Orders.Application.Features.Pedidos.Commands;
using Venice.Orders.Application.Interfaces;
using Venice.Orders.Infrastructure.Persistence.Contexts;
using Venice.Orders.Infrastructure.Persistence.Repositories;
using Venice.Orders.Infrastructure.Services;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining(typeof(CriarPedidoCommand)));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona a defini��o de seguran�a "Bearer".
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' [espa�o] e o seu token JWT.\n\nExemplo: Bearer eyJhbGciOiJI..."
    });

    // Adiciona um requisito de seguran�a global.
    // Isso diz ao Swagger que todos os endpoints (por padr�o) precisam do token "Bearer".
    // � isso que faz o "cadeado" aparecer nos endpoints.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// --- Configura��o da Autentica��o JWT ---
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em dev, pode ser false
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

builder.Services.AddScoped<ITokenService, TokenService>();

// --- Configura��o das Conex�es com os Bancos de Dados ---
var sqlServerConnection = builder.Configuration.GetConnectionString("SqlServer");
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(sqlServerConnection, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, // Tenta conectar at� 5 vezes
            maxRetryDelay: TimeSpan.FromSeconds(30), // O tempo m�ximo de espera entre tentativas
            errorNumbersToAdd: null);
    }));

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("VeniceOrdersItensDB"); // Nome do nosso banco de dados de itens
});

// --- Configura��o do Cache com Redis ---
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// Registra nosso servi�o de cache
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// --- Configura��o das Interfaces e Implementa��es ---
builder.Services.AddScoped<IPedidoWriteRepository, PedidoWriteRepository>();
builder.Services.AddScoped<IPedidoReadRepository, PedidoReadRepository>();

builder.Services.AddSingleton<IMensageriaService>(sp =>
    new RabbitMqService(builder.Configuration.GetConnectionString("RabbitMQ")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SqlServerDbContext>();
        context.Database.Migrate(); // Aplica as migrations pendentes
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro durante a aplica��o da migration.");
    }
}

app.Run();
