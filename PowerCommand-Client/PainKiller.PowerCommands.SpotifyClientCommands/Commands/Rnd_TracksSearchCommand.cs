using PainKiller.PowerCommands.Shared.Extensions;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Randomize a search for tracks that could be used to create a playlist or just added to the queue",
                        useAsync: true,
                         options: "genre|artist|album|track|year|distinct-artists",
                         example: "Random 100 tracks from a Spotify search|random_search|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
// ReSharper disable once InconsistentNaming
public class Rnd_TracksSearchCommand : TracksCommand
{
    public Rnd_TracksSearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

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
            _tracks.Shuffle();
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
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
}