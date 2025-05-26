using Microsoft.AspNetCore.Mvc;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;
using ScreenSoundApi.Request;
using ScreenSoundApi.Response;

namespace ScreenSoundApi.Endpoints
{
    public static class MusicasExtensions
    {
        public static void AddEndpointsMusicas(this WebApplication app)
        {
            app.MapGet("/Musicas", ([FromServices] DAL<Musica> dal) =>
            {
                var response = EntityListToResponseList(dal.Listar());
                return Results.Ok(response);
            });

            app.MapGet("/Musicas/{nome}", ([FromServices] DAL<Musica> dal, string nome) =>
            {
                var musica = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome));

                if (musica is null) { return Results.NotFound(); }

                return Results.Ok(EntityToResponse(musica));
            });

            app.MapPost("/Musicas", ([FromServices] DAL<Musica> dalMusica,[FromServices]DAL<Genero> dalGenero, [FromBody] MusicaRequest musicaRequest) =>
            {
                var musica = new Musica(musicaRequest.Nome)
                {
                    ArtistaId = musicaRequest.ArtistaId,
                    AnoLancamento = musicaRequest.AnoLancamento,
                    Generos = musicaRequest.Generos is not null ? 
                    GeneroRequestConverter(musicaRequest.Generos, dalGenero) : 
                    [],
                };
                dalMusica.Adicionar(musica);
                return Results.Ok();
            });

            app.MapDelete("/Musicas/{id}", ([FromServices] DAL<Musica> dal, int id) =>
            {
                var musica = dal.RecuperarPor(a => a.Id.Equals(id));

                if (musica is null) { return Results.NotFound(id); };
                dal.Deletar(musica);
                return Results.Ok();
            });

            app.MapPut("/Musicas", ([FromServices] DAL<Musica> dal, [FromBody] MusicaRequestEdit musicaRequestEdit) =>
            {
                var musicaParaAtualizar = dal.RecuperarPor(a => a.Id.Equals(musicaRequestEdit.Id));

                if (musicaParaAtualizar is null) { return Results.NotFound(musicaParaAtualizar); };

                musicaParaAtualizar.Nome = musicaRequestEdit.Nome;
                musicaParaAtualizar.AnoLancamento = musicaRequestEdit.AnoLancamento;

                dal.Atualizar(musicaParaAtualizar);

                return Results.Ok();
            });
        }

        private static ICollection<Genero> GeneroRequestConverter(ICollection<GeneroRequest> generosRequests, DAL<Genero> dalGenero)
        {
            var listaDeGeneros=new List<Genero>();

            foreach(var item in generosRequests)
            {
                var entity = RequestToEntity(item);

                var genero = dalGenero.RecuperarPor(g => g.Nome.ToUpper().Equals(item.Nome.ToUpper()));

                if(genero is not null)
                {
                    listaDeGeneros.Add(genero);
                }
                else
                {
                    listaDeGeneros.Add(entity);
                }

               
            }

            return listaDeGeneros;
        }

        private static Genero RequestToEntity(GeneroRequest generoRequest)
        {
            return new Genero() {Nome=generoRequest.Nome,Descricao=generoRequest.Descricao };
        }
        private static ICollection<MusicaResponse> EntityListToResponseList(IEnumerable<Musica> musicas)
        {
            return musicas.Select(a => EntityToResponse(a)).ToList();
        }

        private static MusicaResponse EntityToResponse(Musica musica)
        {
            return new MusicaResponse(musica.Id, musica.Nome!, musica.Artista!.Id, musica.Artista.Nome);
        }
    }
}
