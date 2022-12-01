using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;
public abstract class SpotifyBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected bool NoClient;
    protected static SpotifyDB SpotifyDB = new();
    protected SpotifyClient? Client;
    protected static List<PowerCommandTrack> LastSearch = new();
    protected SpotifyBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        SpotifyDB = StorageService<SpotifyDB?>.Service.GetObject() ?? new SpotifyDB();
        if (!NoClient)
        {
            var token = DialogService.QuestionAnswerDialog("Access token (just hit enter to use current saved one): ");
            if (!string.IsNullOrEmpty(token)) StorageService<Token>.Service.StoreObject(new Token { OathToken = token });
            else token = StorageService<Token>.Service.GetObject().OathToken;
            Client = new SpotifyClient($"{token}");
        }
        return base.InitializeAndValidateInput(input, designAttribute);
    }

    protected void Print(List<PowerCommandTrack> tracks)
    {
        LastSearch.AddRange(tracks);
        var table = tracks.Select(t => new TrackSearchTableItem { Artist = t.Artist, Name = t.Name, ReleaseDate = t.ReleaseDate, PlaylistName = t.PlaylistName, Tags = t.Tags });
        ConsoleTableService.RenderTable(table, this);
        WriteHeadLine($"Found {tracks.Count} tracks");
        Write("You could create a playlist using this search result with the following command:");
        WriteCodeExample("playlist","--create <name> --from-search");
    }
    protected List<PowerCommandTrack> SearchTag(string search) => SpotifyDB.Tracks.Where(t => t.Tags.ToLower().Contains(search)).ToList();
}