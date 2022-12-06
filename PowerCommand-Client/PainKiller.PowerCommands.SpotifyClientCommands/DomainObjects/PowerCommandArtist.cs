using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class PowerCommandArtist
{
    public PowerCommandArtist(){}
    public PowerCommandArtist(FullArtist artist)
    {
        Genres.AddRange(artist.Genres);
        Id = artist.Id;
        Name = artist.Name;
        Popularity = artist.Popularity;
        Uri = artist.Uri;
    }
    public List<string> Genres { get; set; } = new();
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Popularity { get; set; }
    public string Uri { get; set; } = "";
}