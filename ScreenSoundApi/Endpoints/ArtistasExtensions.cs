using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSoundApi.Request;
using ScreenSoundApi.Response;

namespace ScreenSoundApi.Endpoints
{
    public static class ArtistasExtensions
    {

        public static void AddEndpointsArtistas(this WebApplication app)
        {

            app.MapGet("/Artistas", ([FromServices] DAL<Artista> dal) =>
            {
               var response= EntityListToResponseList(dal.Listar());
                return Results.Ok(response);
            });

            app.MapGet("/Artistas/{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
            {
                var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));

                if (artista is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(EntityToResponse(artista));
            });

            app.MapPost("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
            {
                var artista = new Artista(artistaRequest.Nome, artistaRequest.Bio);
                dal.Adicionar(artista);
                return Results.Ok();
            });

            app.MapDelete("/Artistas/{id}", ([FromServices] DAL<Artista> dal, int id) =>
            {
                var artista = dal.RecuperarPor(a => a.Id.Equals(id));

                if (artista is null)
                {
                    return Results.NotFound(id);
                }

                dal.Deletar(artista);
                return Results.Ok();
            });

            app.MapPut("/Artistas", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) =>
            {
                var artistaParaAtualizar = dal.RecuperarPor(a => a.Id.Equals(artistaRequestEdit.Id));

                if (artistaParaAtualizar is null)
                {
                    return Results.NotFound();
                }
                artistaParaAtualizar.Nome = artistaRequestEdit.Nome;
                artistaParaAtualizar.Bio = artistaRequestEdit.Bio;

                dal.Atualizar(artistaParaAtualizar);
                return Results.Ok();
            });    
        }
        private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
        {
            return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
        }
        private static ArtistaResponse EntityToResponse(Artista artista)
        {
            return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
        }
    }
}
