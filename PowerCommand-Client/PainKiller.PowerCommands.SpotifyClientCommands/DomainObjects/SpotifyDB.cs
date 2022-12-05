namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class SpotifyDB
{
    public DateTime Updated { get; set; }
    public List<PowerCommandPlaylist> Playlists { get; set; } = new();
    public List<PowerCommandTrack> Tracks { get; set; } = new();
    public List<PowerCommandArtist> Artists { get; set; } = new();
    public List<string> Genres { get; set; } = new();
}