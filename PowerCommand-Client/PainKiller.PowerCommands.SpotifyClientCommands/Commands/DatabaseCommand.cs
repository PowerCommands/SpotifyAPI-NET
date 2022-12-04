using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: "iron maiden")]
[PowerCommandDesign( description: "Search your locally stored play-lists, default search is on artist, album, title, playlist and tag (in short search all)",
                       arguments: "<search1> <search2> <search2> and so on...",
                         options: "artist|title|album|playlist|!year|tag|begins-with",
                         example: "//Search all|database \"Iron maiden\"|//search playlist|database party --playlist")]
public class DatabaseCommand : SpotifyBaseCommando
{
    public DatabaseCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

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

        SearchPhrase = search;

        if (!Input.MustHaveOneOfTheseOptionCheck(new[] { "artist", "title", "album" }) && string.IsNullOrEmpty(Input.SingleQuote)) return BadParameterError("A search on artist, title or album could not be empty");

        if(HasOption("title")) findings.AddRange(SearchTitle(search));
        else if(HasOption("album")) findings.AddRange(SearchAlbum(search));
        else if(HasOption("tag")) findings.AddRange(SearchTag(search));
        else if(HasOption("playlist")) findings.AddRange(SearchPlaylist(search));
        else if(HasOption("artist")) findings.AddRange(SearchArtist(search));
        else
        {
            var searchAll = new List<PowerCommandTrack>();
            searchAll.AddRange(SearchTitle(search));
            searchAll.AddRange(SearchAlbum(search));
            searchAll.AddRange(SearchTag(search));
            searchAll.AddRange(SearchPlaylist(search));
            searchAll.AddRange(SearchArtist(search));
            foreach (var track in searchAll.Where(track => findings.All(f => f.Id != track.Id))) findings.Add(track);
        }
        if(HasOption("year")) findings = findings.Where(f => f.ReleaseYear == Input.OptionToInt("year")).ToList();
        if(HasOption("begins-with")) findings = findings.Where(f => f.Artist.ToLower().StartsWith(search) || f.Name.ToLower().StartsWith(search) || f.AlbumName.ToLower().StartsWith(search)).ToList();
        Print(findings);

        EnableLog();
        return Ok();
    }

    private List<PowerCommandTrack> SearchArtist(string search) => SpotifyDB.Tracks.Where(t => t.Artist.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchTitle(string search) => SpotifyDB.Tracks.Where(t => t.Name.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchAlbum(string search) => SpotifyDB.Tracks.Where(t => t.AlbumName.ToLower().Contains(search)).ToList();
    private List<PowerCommandTrack> SearchPlaylist(string search) => SpotifyDB.Tracks.Where(t => t.PlaylistName.ToLower().Contains(search)).ToList();
}