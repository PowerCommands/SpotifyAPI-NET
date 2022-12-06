using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a track list, default random count is 100 tracks",
                        useAsync: true,
                         options: "all-local|all-related|!count|queue",
                         example: "Random 100 tracks from your play-lists|random|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
public class RandomCommand : QueueCommand
{
    public RandomCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        var take = Input.OptionToInt("count");
        take = take == 0 ? 100 : take;
        var queue = HasOption("queue");
        LastSearchedTracks.Clear();
        IPlaylistAlgorithm algorithm;
        if(HasOption("all-local")) algorithm= new AllLocal(this, take, queue);
        else algorithm = new AllRelated(this, take, queue);
        try
        {
            await algorithm.RunAsync();
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
        }
        return Ok();
    }
}