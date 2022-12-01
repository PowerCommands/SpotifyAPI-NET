namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class TrackSearchTableItem
{
    public string Artist { get; set; } = "";
    public string Name { get; set; } = default!;
    public string ReleaseDate { get; set; } = "";
    public int ReleaseYear => int.TryParse($"{ReleaseDate}".Length > 3 ? ReleaseDate.Substring(0,4) : "0", out var year) ? year : 0;
    public string PlaylistName { get; set; } = "";
    public string Tags { get; set; } = "";
}