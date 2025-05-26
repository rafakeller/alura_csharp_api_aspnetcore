using System.ComponentModel.DataAnnotations;

namespace ScreenSoundApi.Request
{
    public record MusicaRequest([Required] string Nome, [Required] int ArtistaId, int AnoLancamento, ICollection<GeneroRequest> Generos=null);
}
