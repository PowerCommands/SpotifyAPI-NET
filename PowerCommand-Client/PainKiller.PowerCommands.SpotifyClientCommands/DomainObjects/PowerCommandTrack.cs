using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class PowerCommandTrack
{
    public PowerCommandTrack(){}
    public PowerCommandTrack(FullTrack track, string playlistName)
    {
        Artist = track.Artists.FirstOrDefault() == null ? "?" : track.Artists.First().Name;
        Id = track.Id;
        DurationMs = track.DurationMs;
        IsLocal = track.IsLocal;
        Name = track.Name;
        Popularity = track.Popularity;
        TrackNumber = track.TrackNumber;
        Uri = track.Uri;
        IsPlayable = track.IsPlayable;
        AlbumName = track.Album.Name;
        ReleaseDate = track.Album.ReleaseDate;
        PlaylistName = playlistName;
    }
    public string Artist { get; set; } = "";
    public int DurationMs { get; set; }
    public string Id { get; set; } = "";
    public bool IsPlayable { get; set; }
    public string Name { get; set; } = default!;
    public int Popularity { get; set; }
    public int TrackNumber { get; set; }
    public string Uri { get; set; } = default!;
    public bool IsLocal { get; set; }
    public string AlbumName { get; set; } = "";
    public string PlaylistName { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public string Tags { get; set; } = "";
    public int ReleaseYear => int.TryParse($"{ReleaseDate}".Length > 3 ? ReleaseDate.Substring(0,4) : "0", out var year) ? year : 0;
}