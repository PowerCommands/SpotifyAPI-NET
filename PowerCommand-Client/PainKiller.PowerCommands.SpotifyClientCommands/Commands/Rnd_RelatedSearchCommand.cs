using PainKiller.PowerCommands.SpotifyClientCommands.Extensions;
using PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a search that could be used to create a playlist or just added to the queue",
                        useAsync: true,
                         options: "",
                         example: "Random 100 tracks from your play-lists|random_search|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
// ReSharper disable once InconsistentNaming
public class Rnd_RelatedSearchCommand : SpotifyBaseCommando
{
    public Rnd_RelatedSearchCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        DisableLog();
        try
        {
            var algoritm = new RelatedSearchAlgorithm(this);
            var tracks = await algoritm.FindTracksAsync();
            await tracks.AddToQueueOrJustShowResult(this);
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }
        EnableLog();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
}