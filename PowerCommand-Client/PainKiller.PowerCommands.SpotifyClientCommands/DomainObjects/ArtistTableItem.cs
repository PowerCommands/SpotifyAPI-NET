namespace PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

public class ArtistTableItem
{
    public ArtistTableItem(){}

    public ArtistTableItem(PowerCommandArtist artist)
    {
        Genres = string.Join(",", artist.Genres);
        Id  = artist.Id;
        Name = artist.Name;
        Popularity = artist.Popularity;
    }

    public int Index { get; set; }
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Genres { get; set; } = ""; 
    public int Popularity { get; set; }
}