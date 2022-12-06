using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a search that could be used to create a playlist or just added to the queue",
                        useAsync: true,
                         options: "genre|artist|album|track|year|!count|queue",
                         example: "Random 100 tracks from your play-lists|random_search|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
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

        SearchPhrase = Input.BuildSearchPhrase();
        try
        {
            var searchResponse = await Client!.Search.Item(new SearchRequest(SearchRequest.Types.All, SearchPhrase));
            EnumerateTracks(searchResponse.Tracks);
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