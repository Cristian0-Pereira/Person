using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Person.Data;
using Person.Models;

namespace Person.Routes;

public static class PersonRoute
{
    public static void PersonRoutes(this WebApplication app)
    {
        var route = app.MapGroup("person");

        // Rota POST para adicionar uma pessoa
        route.MapPost("", async (PersonRequest req, PersonContext context) =>
        {
            if (string.IsNullOrWhiteSpace(req.name))
                return Results.BadRequest("O nome não pode ser vazio.");

            try
            {
                var person = new PersonModel(req.name);
                await context.AddAsync(person);
                await context.SaveChangesAsync();
                return Results.Created($"/person/{person.Id}", person);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao adicionar a pessoa: {ex.Message}");
            }
        });

        // Rota GET para obter todas as pessoas
        route.MapGet("", async (PersonContext context) => 
        {
            try
            {
                var people = await context.People.ToListAsync();
                return Results.Ok(people);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao buscar as pessoas: {ex.Message}");
            }
        });

        // Rota PUT para atualizar uma pessoa existente
        route.MapPut("{id:guid}",
        async (Guid id, PersonRequest req, PersonContext context) =>
        {
            var person = await context.People.FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                return Results.NotFound($"Pessoa com Id {id} não encontrada.");

            // Usando reflexão para modificar a propriedade Name
            person.ChangeName(req.name);

            try
            {
                // Salvando as mudanças no banco de dados
                await context.SaveChangesAsync();
                return Results.Ok(person); // Retorna a pessoa atualizada
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao atualizar a pessoa: {ex.Message}");
            }
        });
    }
}

