using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Search the Spotify catalog",
                       arguments: "<search1> <search2> <search2> and so on...",
                         options: "!genre|artist|track|album|!year|begins-with|!page-count",
                        useAsync: true,
                         example: "search \"balls to the wall\" --title")]
public class SearchCommand : SpotifyBaseCommando
{
    int pageCounter = 0;
    private int maxPageCount = 3;
    private readonly List<PowerCommandTrack> _tracks = new();
    public SearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        DisableLog();
        _tracks.Clear();
        LastSearchedTracks.Clear();

        
        SearchPhrase = Input.BuildSearchPhrase();

        var searchType = SearchRequest.Types.All;

        if(HasOption("track")) searchType = SearchRequest.Types.Track;
        else if(HasOption("album")) searchType = SearchRequest.Types.Album;
        else if(HasOption("playlist")) searchType = SearchRequest.Types.Playlist;
        else if(HasOption("artist")) searchType = SearchRequest.Types.Artist;
        pageCounter = 0;
        if (HasOption("page-count")) maxPageCount = Input.OptionToInt("page-count");

        var searchResponse = await Client!.Search.Item(new SearchRequest(searchType, SearchPhrase));
        
        switch (searchType)
        {
            case SearchRequest.Types.Album:
                EnumerateAlbums(searchResponse.Albums);
                break;
            case SearchRequest.Types.Track:
                EnumerateTracks(searchResponse.Tracks);
                if (HasOption("artist")) LastSearchedTracks = LastSearchedTracks.Where(t => t.Artist.ToLower().Contains(GetOptionValue("artist").ToLower())).ToList();
                Print(_tracks);
                break;
            case SearchRequest.Types.Artist:
                EnumerateArtists(searchResponse.Artists);
                break;
            default:
                EnumerateTracks(searchResponse.Tracks);
                Print(_tracks);
                break;
        }
        Write(ConfigurationGlobals.Prompt);
        EnableLog();
        return Ok();
    }
    private void EnumerateTracks(Paging<FullTrack, SearchResponse> page)
    {
        if(page.Items == null) return;
        foreach (var item in page.Items)
        {
            if (item is not { } track) continue;
            var pcTrack = new PowerCommandTrack(track, "");
            _tracks.Add(pcTrack);
        }
        if(page.Items.Count < 20) return;
        var nextPage = Client?.NextPage(page);
        if(nextPage == null || pageCounter > maxPageCount) return;
        pageCounter++;
        EnumerateTracks(nextPage.Result.Tracks);
    }

    private void EnumerateAlbums(Paging<SimpleAlbum, SearchResponse> page)
    {
        if(page.Items == null) return;
        foreach (var item in page.Items)
        {
            if (item is not { } simpleAlbum) continue;
            Console.WriteLine($"{simpleAlbum.Artists.First().Name} {simpleAlbum.Name} {item.ReleaseDate}");
        }
        if(page.Items.Count < 20) return;
        var nextPage = Client?.NextPage(page);
        if(nextPage == null || pageCounter > maxPageCount) return;
        pageCounter++;
        EnumerateAlbums(nextPage.Result.Albums);
    }
    private void EnumeratePlaylists(Paging<SimplePlaylist, SearchResponse> page)
    {
        if(page.Items == null) return;
        foreach (var item in page.Items)
        {
            if (item is not { } simplePlaylist) continue;
            if (item.Tracks.Items != null) Console.WriteLine($"{simplePlaylist.Name} {item.Tracks.Items.Count}");
        }
    }
    private void EnumerateArtists(Paging<FullArtist, SearchResponse> page)
    {
        if(page.Items == null) return;
        LastSearchedArtists.Clear();
        var artistTable = new List<PowerCommandArtist>();
        foreach (var item in page.Items)
        {
            if (item is not { } fullArtist) continue;
            artistTable.Add(new PowerCommandArtist(item));
        }
        Print(artistTable);
    }
}