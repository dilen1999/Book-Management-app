using BookMS.Application.Abstractions;
using BookMS.Application.Common.Behaviors;
using BookMS.Application.Common.Options;
using BookMS.Application.Services;
using BookMS.Infrastructure.Persistence;
using BookMS.Infrastructure.Security;
using BookMS.WebApi.Middleware;                 
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigin = builder.Configuration["Cors:Frontend"] ?? "http://localhost:5173";

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p =>
        p.WithOrigins(allowedOrigin)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
    );
});
// ------------------ Services ------------------

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
       .EnableDetailedErrors()      
);

// Expose DbContext via app abstraction (Clean Architecture)
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// MediatR / CQRS + FluentValidation + AutoMapper (scan Application assembly)
var appAssembly = typeof(BookMS.Application.Mapping.MappingProfile).Assembly;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
builder.Services.AddValidatorsFromAssembly(appAssembly);
builder.Services.AddAutoMapper(appAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

// ---------- JWT ----------
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher<BookMS.Domain.Entities.Users>, PasswordHasher<BookMS.Domain.Entities.Users>>();
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection("Google"));

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });


// Pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>)); // wraps *commands* in a transaction

// Global exception middleware
builder.Services.AddTransient<GlobalExceptionMiddleware>();

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookMS API",
        Version = "v1"
    });

    // ?? Add JWT Bearer Auth
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: \"Bearer eyJhbGciOi...\""
    });

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
            Array.Empty<string>()
        }
    });
});

// ------------------ Build app ------------------
var app = builder.Build();

// Apply pending migrations automatically on startup (dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    
    // Seed default roles if they don't exist
    if (!db.Roles.Any())
    {
        db.Roles.AddRange(
            new BookMS.Domain.Entities.Roles { Name = "User" },
            new BookMS.Domain.Entities.Roles { Name = "Admin" },
            new BookMS.Domain.Entities.Roles { Name = "SuperAdmin" }
        );
        db.SaveChanges();
    }
}

// ------------------ Middleware pipeline ------------------

// Global exception handler first so it can catch everything below
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Optional HTTPS redirection (keep if you need it)
app.UseHttpsRedirection();
app.UseCors("frontend");

// If you add auth later, these would go here:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
