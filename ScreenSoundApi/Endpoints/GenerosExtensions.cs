using ScreenSound.Banco;
using Microsoft.AspNetCore.Mvc;
using ScreenSound.Shared.Modelos.Modelos;
using ScreenSoundApi.Response;
using Microsoft.IdentityModel.Tokens;
using ScreenSoundApi.Request;

namespace ScreenSoundApi.Endpoints
{
    public static class GenerosExtensions
    {
        public static void AddEndpointsGeneros(this WebApplication app)
        {
            app.MapGet("/Generos", ([FromServices] DAL<Genero> dal) =>
            {
                var response = EntityListToResponse(dal.Listar());
                return Results.Ok(response);
            });

            app.MapGet("/Generos/{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
            {
                var genero=dal.RecuperarPor(a=>a.Nome.ToUpper().Equals(nome.ToUpper()));
                if(genero is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(EntityToResponse(genero));
            });

            app.MapPost("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
            {
                dal.Adicionar(RequestToEntity(generoRequest));
                return Results.Ok();
            });

            app.MapDelete("/Generos/{id}", ([FromServices] DAL<Genero> dal,int id)=>
            { 
                var genero= dal.RecuperarPor(a=>a.Id.Equals(id));
                if(genero is null)
                {
                    return Results.NotFound("Genero para exclusão não encontrado");
                }

                dal.Deletar(genero);

                return Results.NoContent();
            });

            app.MapPut("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequestEdit generoRequestEdit) =>
            {
               var generoParaAtualizar = dal.RecuperarPor(a=>a.Id.Equals(generoRequestEdit.Id));
                if(generoParaAtualizar is null)
                {
                    return Results.NotFound();
                }

                generoParaAtualizar.Nome=generoRequestEdit.Nome;
                generoParaAtualizar.Descricao=generoRequestEdit.Descricao;  

                dal.Atualizar(generoParaAtualizar);

                return Results.Ok();
            });
        }
        private static ICollection<GenerosResponse> EntityListToResponse(IEnumerable<Genero> listaDeGeneros)
        {
            return listaDeGeneros.Select(a => EntityToResponse(a)).ToList();
        }

        private static GenerosResponse EntityToResponse(Genero genero)
        {
            return new GenerosResponse(genero.Id,genero.Nome!, genero.Descricao!);
        }
        private static Genero RequestToEntity(GeneroRequest generoRequest)
        {
            return new Genero()
            {
                Nome = generoRequest.Nome,
                Descricao = generoRequest.Descricao,
            };
        }
    }
}
