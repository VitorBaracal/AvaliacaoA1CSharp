using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

builder.Services.AddCors(options =>
    options.AddPolicy("Acesso Total",
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);


var app = builder.Build();

app.UseCors("Acesso Total");

app.MapGet("/", () => "Vitor Baraçal Guimarães");

//ENDPOINTS DE TAREFA
//GET: http://localhost:5273/api/chamado/listar
app.MapGet("/api/chamado/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Chamados.Any())
    {
        return Results.Ok(ctx.Chamados.ToList());
    }
    return Results.NotFound("Nenhum chamado encontrada");
});

//POST: http://localhost:5273/api/chamado/cadastrar
app.MapPost("/api/chamado/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Chamado chamado) =>
{
    ctx.Chamados.Add(chamado);
    ctx.SaveChanges();
    return Results.Created("", chamado);
});

//PATCH: http://localhost:5273/api/chamado/alterar
app.MapPatch("/api/chamado/alterar", ([FromServices] AppDataContext ctx, [FromBody] Chamado chamadoReq) =>
{
    Chamado? chamado = ctx.Chamados.Find(chamadoReq.ChamadoId);
    
    if (chamado == null)
    {
        return Results.NotFound("Chamado não encontrado");
    }

    if (chamado.Status == "Aberto")
    {
        chamado.Status = "Em atendimento";
    }
    else if (chamado.Status == "Em atendimento")
    {
        chamado.Status = "Resolvido";
    }

    ctx.SaveChanges();
    return Results.Ok(chamado);
});

//GET: http://localhost:5273/api/chamado/naoresolvido
app.MapGet("/api/chamado/naoresolvido", ([FromServices] AppDataContext ctx) =>
{
    var chamadosNaoResolvidos = ctx.Chamados.Where(c => c.Status == "Aberto" || c.Status == "Em atendimento").ToList();
    return Results.Ok(chamadosNaoResolvidos);
});

//GET: http://localhost:5273/api/chamado/resolvidos
app.MapGet("/api/chamado/resolvidos", ([FromServices] AppDataContext ctx) =>
{
    var chamadosResolvidos = ctx.Chamados.Where(c => c.Status == "Resolvido").ToList();
    return Results.Ok(chamadosResolvidos);
});

app.Run();
