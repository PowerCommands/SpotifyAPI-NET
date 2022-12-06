using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;
public abstract class SpotifyBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    public string SearchPhrase = "";
    public static LastSearchType LastSearchType = LastSearchType.None;
    public bool NoClient;
    public static SpotifyDB SpotifyDB = new();
    public SpotifyClient? Client;
    public static List<PowerCommandTrack> LastSearchedTracks = new();
    public static List<PowerCommandArtist> LastSearchedArtists = new();
    protected SpotifyBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        SpotifyDB = StorageService<SpotifyDB?>.Service.GetObject() ?? new SpotifyDB();
        if (NoClient) return base.InitializeAndValidateInput(input, designAttribute);
        var token = StorageService<Token>.Service.GetObject().OathToken;
        Client = new SpotifyClient($"{token}");
        return base.InitializeAndValidateInput(input, designAttribute);
    }

    public void Print(List<PowerCommandTrack> tracks)
    {
        var take = Input.OptionToInt("take");
        LastSearchType = LastSearchType.Track;
        LastSearchedTracks.AddRange(tracks);
        var table = tracks.Select((t, index) => new TrackSearchTableItem { Artist = t.Artist, Name = t.Name, ReleaseDate = t.ReleaseDate, PlaylistName = t.PlaylistName, Popularity = t.Popularity, Tags = t.Tags, Index = index++ }).Take(take == 0 ? 1000 : take);
        ConsoleTableService.RenderTable(table, this);
        WriteHeadLine($"Found {tracks.Count} tracks with search phrase {SearchPhrase}");
        Write("You could create a playlist using this search result with the following command:");
        WriteCodeExample("playlist","--create \"<name of your playlist>\"");
    }

    protected void Print(List<PowerCommandArtist> artists)
    {
        var take = Input.OptionToInt("take", 1000);
        LastSearchType = LastSearchType.Artist;
        LastSearchedArtists.AddRange(artists);
        var table = artists.Select((a, index) => new ArtistTableItem(a){Index = index++}).Take(take == 0 ? 1000 : take).ToList();
        ConsoleTableService.RenderTable(table, this);
        WriteHeadLine($"Found {table.Count} artist with search phrase {SearchPhrase}");
        Write("You could use an artist from this search to find tracks, albums or related artists:");
        WriteCodeExample("artist","--related 0");
        WriteCodeExample("artist","--tracks 0");
    }

    protected void Print(List<PowerCommandPlaylist> playlists)
    {
        var take = Input.OptionToInt("take", 1000);
        LastSearchType = LastSearchType.Playlist;
        var table = playlists.Select((t, index) => new PlaylistSearchTableItem { Name = t.Name, Id = t.Id, TrackCount = t.Tracks.Count, Index = index++ }).Take(take == 0 ? 1000 : take);
        ConsoleTableService.RenderTable(table, this);
        WriteHeadLine($"Found {playlists.Count} play-lists");
        Write("You could add a tag to a playlist using the index like this:");
        WriteCodeExample("tag","--create --playlist 0");
        Write(ConfigurationGlobals.Prompt);
    }
    protected List<PowerCommandTrack> SearchTag(string search) => SpotifyDB.Tracks.Where(t => t.Tags.ToLower().Contains(search)).ToList();


}