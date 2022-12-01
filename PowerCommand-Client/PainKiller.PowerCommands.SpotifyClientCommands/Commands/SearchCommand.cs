using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Search Spotify catalog or your locally stored playlists",
                          quotes: "<search>",
                         options: "title|artist|album|!year",
                         example: "search \"Iron maiden\"")]
public class SearchCommand : SpotifyBaseCommando
{
    public SearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        NoClient = true;
        return base.InitializeAndValidateInput(input, designAttribute);
    }

    public override RunResult Run()
    {
        DisableLog();
        var findings = new List<PowerCommandTrack>();
        var search = string.IsNullOrEmpty(Input.SingleQuote) ? Input.SingleArgument : Input.SingleQuote;

        if(HasOption("artist")) findings.AddRange(SearchArtist(search));
        else if(HasOption("album")) findings.AddRange(SearchAlbum(search));
        else if(HasOption("title")) findings.AddRange(SearchTitle(search));

        if(HasOption("year")) findings = findings.Where(f => f.ReleaseYear == Input.OptionToInt("year")).ToList();

        Print(findings);

        EnableLog();
        return Ok();
    }

    private List<PowerCommandTrack> SearchArtist(string search) => SpotifyDB.Tracks.Where(t => t.Artist.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchTitle(string search) => SpotifyDB.Tracks.Where(t => t.Name.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchAlbum(string search) => SpotifyDB.Tracks.Where(t => t.AlbumName.ToLower().Contains(search)).ToList();

    private void Print(List<PowerCommandTrack> tracks)
    {
        var table = tracks.Select(t => new TrackSearchTableItem { Artist = t.Artist, Name = t.Name, ReleaseDate = t.ReleaseDate });
        ConsoleTableService.RenderTable(table, this);
        WriteHeadLine($"Found {tracks.Count} tracks");
    }

}