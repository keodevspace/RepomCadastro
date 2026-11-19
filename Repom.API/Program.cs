using Microsoft.EntityFrameworkCore;
using Repom.API.Middlewares;
using Repom.Application.Services;
using Repom.Domain.Interfaces;
using Repom.Infrastructure.Data;
using Repom.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Banco de Dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=repom_frota.db"));

// Injeção de Dependência (Repositories & Services)
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<PessoaService>();
builder.Services.AddScoped<VeiculoService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Garante criação automática do banco SQLite na primeira execução
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve o arquivo único HTML da interface gráfica em wwwroot/index.html
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();