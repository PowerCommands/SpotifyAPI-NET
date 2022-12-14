using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Search tracks",
                         options: "genre|artist|album|track|year|distinct-artists",
                        useAsync: true,
                         example: "tracks --genre \"hard rock\"")]
public class TracksCommand : SpotifyBaseCommando
{
    protected int _pageCounter = 0;
    protected int MaxPageCount = 10;
    protected readonly List<PowerCommandTrack> _tracks = new();
    public TracksCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        DisableLog();
        _tracks.Clear();
        LastSearchedTracks.Clear();
        _pageCounter = 0;
        SearchPhrase = Input.BuildSearchPhrase();
        try
        {
            var searchResponse = await Client!.Search.Item(new SearchRequest(SearchRequest.Types.Track, SearchPhrase));
            EnumerateTracks(searchResponse.Tracks);
            if (HasOption("distinct-artists"))
            {
                var distinctArtistTracks = new List<PowerCommandTrack>();
                foreach (var track in _tracks)
                {
                    if(distinctArtistTracks.Any(t => t.ArtistId == track.ArtistId)) continue;
                    distinctArtistTracks.Add(track);
                }
                _tracks.Clear();
                _tracks.AddRange(distinctArtistTracks);
            }
            Print(_tracks);
        }
        catch (Exception ex)
        {
            WriteLine($"Search phrase:{SearchPhrase}");
            WriteError(ex.Message);
        }
        EnableLog();
        return Ok();
    }
    protected void EnumerateTracks(Paging<FullTrack, SearchResponse> page)
    {
        if(page.Items == null || _pageCounter > MaxPageCount) return;
        foreach (var item in page.Items)
        {
            if (item is not { } track) continue;
            var pcTrack = new PowerCommandTrack(track, "");
            _tracks.Add(pcTrack);
        }
        if(page.Items.Count < 20) return;
        var nextPage = Client?.NextPage(page);
        if(nextPage == null || _pageCounter > MaxPageCount) return;
        _pageCounter++;
        EnumerateTracks(nextPage.Result.Tracks);
    }
}