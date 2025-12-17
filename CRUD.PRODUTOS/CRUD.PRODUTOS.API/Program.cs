using CRUD.PRODUTOS.DATA.Data;
using CRUD.PRODUTOS.DATA.Repositories;
using CRUD.PRODUTOS.INTERFACES;
using CRUD.PRODUTOS.SERVICES;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers (API)
builder.Services.AddControllers();

// Dependency Injection
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// DbContext
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Swagger (recomendado)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = ""; 
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// API endpoints
app.MapControllers();

app.Run();