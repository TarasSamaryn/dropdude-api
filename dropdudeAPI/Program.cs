using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinefieldServer.Data;
using MinefieldServer.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --- Слухаємо на порту 10000 
builder.WebHost.UseUrls("http://*:10000");

// --- Logging ---
builder.Logging.ClearProviders();
builder.Logging.AddProvider(new MyInMemoryLoggerProvider());
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// --- Configuration ---
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? builder.Configuration["DATABASE_URL"];

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");
}

// --- PostgreSQL ---
builder.Services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(connectionString));

// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minefield API V1", 
        Version = "v1"
    });

    // замінили ApiKey на Http-схему Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement 
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
            Array.Empty<string>()
        }
    });
});

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// --- JWT Authentication ---
string? jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key not configured.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// --- Authorization policies ---
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireClaim("isAdmin", "True"));
});

WebApplication app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// --- Swagger middleware ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minefield API V1");
    c.RoutePrefix = ""; // відкривається на корені: http://localhost:10000/
});

// 1) Ендпоінт для отримання логів у форматі JSON
app.MapGet("/logs", () =>
{
    string[] logs = MyInMemoryLogger.GetLogs();
    return Results.Ok(logs);
});

// 2) Лог-вівер — віддає статичний HTML
app.MapGet("/logview", async ctx =>
{
    string webRoot = app.Environment.WebRootPath ?? "wwwroot";
    string file = Path.Combine(webRoot, "admin", "logview.html");
    
    if (!File.Exists(file))
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsync("Log viewer not found");
        return;
    }
    
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.SendFileAsync(file);
});

app.MapGet("/leaderboard", async ctx =>
{
    string webRoot = app.Environment.WebRootPath ?? "wwwroot";
    string file = Path.Combine(webRoot, "admin", "leaderboard.html");
    
    if (!File.Exists(file))
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsync("Leaderboard page not found");
        return;
    }
    
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.SendFileAsync(file);
});

app.MapGet("/admin", async ctx =>
{
    string file = Path.Combine(app.Environment.WebRootPath, "admin", "index.html");
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.SendFileAsync(file);
});

// 4) Web API контролери
app.MapControllers();

// 5) Автоматичні міграції
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
