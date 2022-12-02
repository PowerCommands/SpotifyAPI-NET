namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
public class Tagged
{
    public List<PowerCommandTrack> Tracks { get; set; } = new();
    public List<PowerCommandPlaylist> Playlists { get; set; } = new();
}