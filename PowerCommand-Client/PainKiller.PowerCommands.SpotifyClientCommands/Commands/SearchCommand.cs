using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Search Spotify catalog or your locally stored play-lists, default search is on artist",
                       arguments: "<search1> <search2>",
                         options: "title|album|!year|!tag|artist|begins-with",
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
        
        LastSearchedTracks.Clear();

        var findings = new List<PowerCommandTrack>();
        var search = Input.SingleQuote.ToLower();
        if (string.IsNullOrEmpty(search)) search = string.Join(' ', Input.Arguments);


        if (!Input.MustHaveOneOfTheseOptionCheck(new[] { "artist", "title", "album" }) && string.IsNullOrEmpty(Input.SingleQuote)) return BadParameterError("A search on artist, title or album could not be empty");

        if(HasOption("title")) findings.AddRange(SearchTitle(search));
        else if(HasOption("album")) findings.AddRange(SearchAlbum(search));
        else if(HasOption("tag")) findings.AddRange(SearchTag(GetOptionValue("tag")));
        else findings.AddRange(SearchArtist(search));
        
        if(HasOption("year")) findings = findings.Where(f => f.ReleaseYear == Input.OptionToInt("year")).ToList();
        if(HasOption("begins-with")) findings = findings.Where(f => f.Artist.ToLower().StartsWith(search) || f.Name.ToLower().StartsWith(search) || f.AlbumName.ToLower().StartsWith(search)).ToList();
        Print(findings);

        EnableLog();
        return Ok();
    }

    private List<PowerCommandTrack> SearchArtist(string search) => SpotifyDB.Tracks.Where(t => t.Artist.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchTitle(string search) => SpotifyDB.Tracks.Where(t => t.Name.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchAlbum(string search) => SpotifyDB.Tracks.Where(t => t.AlbumName.ToLower().Contains(search)).ToList();
}