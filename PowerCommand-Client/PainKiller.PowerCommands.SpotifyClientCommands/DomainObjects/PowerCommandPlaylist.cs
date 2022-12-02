using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class PowerCommandPlaylist
{
    public PowerCommandPlaylist() { }
    public PowerCommandPlaylist(SimplePlaylist simplePlaylist)
    {
        Id = simplePlaylist.Id;
        Name = simplePlaylist.Name;
        Public = simplePlaylist.Public.GetValueOrDefault();
    }
    public string Id { get; set; } = "default";
    public string Name { get; set; } = "default";
    public bool Public { get; set; }
    public string Tags { get; set; } = "";
    public List<PowerCommandTrack> Tracks { get; set; } = new();
}