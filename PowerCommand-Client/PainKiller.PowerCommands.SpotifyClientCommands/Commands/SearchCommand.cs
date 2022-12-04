using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: "abba")]
[PowerCommandDesign( description: "Search the Spotify catalog",
                       arguments: "<search1> <search2> <search2> and so on...",
                         options: "artist|track|album|!year|!genre|begins-with|!page-count",
                        useAsync: true,
                         example: "search \"balls to the wall\" --title")]
public class SearchCommand : SpotifyBaseCommando
{
    int pageCounter = 0;
    private int maxPageCount = 3;
    public SearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        DisableLog();

        var search = Input.SingleQuote.ToLower();
        if (string.IsNullOrEmpty(search)) search = string.Join(' ', Input.Arguments);
        SearchPhrase = search;

        var searchType = SearchRequest.Types.All;

        if(HasOption("track")) searchType = SearchRequest.Types.Track;
        else if(HasOption("album")) searchType = SearchRequest.Types.Album;
        else if(HasOption("playlist")) searchType = SearchRequest.Types.Playlist;
        else if(HasOption("artist")) searchType = SearchRequest.Types.Artist;
        pageCounter = 0;
        if (HasOption("page-count")) maxPageCount = Input.OptionToInt("page-count");

        if(HasOption("genre")) search = $"{search} genre:{GetOptionValue("genre")}";

        var searchResponse = await Client!.Search.Item(new SearchRequest(searchType, search));
        
        switch (searchType)
        {
            case SearchRequest.Types.Album:
                EnumerateAlbums(searchResponse.Albums);
                break;
            case SearchRequest.Types.Artist:
                EnumerateArtists(searchResponse.Artists);
                break;
            case SearchRequest.Types.Track:
                EnumerateTracks(searchResponse.Tracks);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
            Console.WriteLine($"{track.Artists.First().Name} {track.Name} {item.Popularity}");
            var pcTrack = new PowerCommandTrack(track, "");
            LastSearchedTracks.Add(pcTrack);
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