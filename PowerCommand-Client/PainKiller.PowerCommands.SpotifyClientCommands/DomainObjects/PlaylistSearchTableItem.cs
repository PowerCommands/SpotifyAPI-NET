namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class PlaylistSearchTableItem
{
    public int Index { get; set; }
    public string Id { get; set; } = "default";
    public string Name { get; set; } = "default";
    public int TrackCount { get; set; }
    public string Tags { get; set; } = "";
}