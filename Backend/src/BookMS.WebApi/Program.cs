using BookMS.Application.Abstractions;
using BookMS.Application.Common.Behaviors;
using BookMS.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// expose DbContext via app abstraction
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// MediatR / CQRS
var appAssembly = typeof(BookMS.Application.Mapping.MappingProfile).Assembly;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(appAssembly));
builder.Services.AddValidatorsFromAssembly(appAssembly);
builder.Services.AddAutoMapper(appAssembly);

// pipeline: validation
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MVC & Swagger
//builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // applies pending migrations automatically
}

app.MapControllers();
app.Run();


