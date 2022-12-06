using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a track list, default random count is 100 tracks",
                        useAsync: true,
                         options: "local|related|!count|queue",
                         example: "Random 100 tracks from your play-lists|random_simple|//Random 50 tracks|random_simple --count 50|//Random 100 track and add them to queue|random_simple --queue")]
// ReSharper disable once InconsistentNaming
public class Rnd_SimpleCommand : QueueCommand
{
    public Rnd_SimpleCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        var take = Input.OptionToInt("count");
        take = take == 0 ? 100 : take;
        var queue = HasOption("queue");
        LastSearchedTracks.Clear();
        IPlaylistAlgorithm algorithm;
        if(HasOption("related")) algorithm= new AllRelated(this, take, queue);
        else algorithm = new AllLocal(this, take, queue);
        try
        {
            await algorithm.RunAsync();
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
}