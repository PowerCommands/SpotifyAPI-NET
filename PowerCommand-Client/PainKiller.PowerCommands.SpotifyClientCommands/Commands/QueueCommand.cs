namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Add a track or playlist to the queue from a previous search",
                        useAsync: true,
                         options: "add",
                         example: "queue 0")]
public class QueueCommand : PlayingCommand
{
    public QueueCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        if (HasOption("add")) await AddTracksToQueue();
        else await ShowQueue();
        Thread.Sleep(500);
        await ShowCurrentlyPlayingTrack();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
    protected async Task AddTracksToQueue()
    {
        var index =  int.TryParse(Input.SingleArgument, out var idx) ? idx : -1;
        if (index > -1)
        {
            await AddTracksToQueue(LastSearchedTracks[index]);
            return;
        }
        foreach (var track in LastSearchedTracks) await AddTracksToQueue(track);
    }
}