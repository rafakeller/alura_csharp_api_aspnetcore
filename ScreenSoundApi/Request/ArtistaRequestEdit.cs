﻿namespace ScreenSoundApi.Request
{
    public record ArtistaRequestEdit( int Id,string Nome, string Bio):ArtistaRequest(Nome, Bio);

}
