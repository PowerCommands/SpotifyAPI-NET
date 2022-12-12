using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a search that could be used to create a playlist or just added to the queue",
                        useAsync: true,
                         options: "genre|artist|album|track|year|distinct-artists",
                         example: "Random 100 tracks from a Spotify search|random_search|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
// ReSharper disable once InconsistentNaming
public class Rnd_SearchCommand : TracksCommand
{
    public Rnd_SearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        DisableLog();
        _tracks.Clear();
        LastSearchedTracks.Clear();
        _pageCounter = 0;
        MaxPageCount = 20;

        SearchPhrase = Input.BuildSearchPhrase();
        try
        {
            var searchResponse = await Client!.Search.Item(new SearchRequest(SearchRequest.Types.All, SearchPhrase));
            EnumerateTracks(searchResponse.Tracks);
            _tracks.Shuffle();
            if (!HasOption("distinct-artists")) Print(_tracks.Take(Take).ToList());
            else
            {
                var distinctArtist = new List<PowerCommandTrack>();
                foreach (var powerCommandTrack in _tracks)
                {
                    if (distinctArtist.All(a => a.ArtistId != powerCommandTrack.ArtistId))
                    {
                        distinctArtist.Add(powerCommandTrack);
                        if (AddToQueue) await AddTracksToQueue(powerCommandTrack);
                    }
                }
                if(!AddToQueue) Print(distinctArtist.Take(Take).ToList());
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Search phrase:{SearchPhrase}");
            WriteError(ex.Message);
        }
        EnableLog();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
}