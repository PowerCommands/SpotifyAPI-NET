using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Add a track or playlist to the queue from a previous search",
                        useAsync: true,
                         example: "queue 0")]
public class QueueCommand : PlayingCommand
{
    public QueueCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        if (string.IsNullOrEmpty(Input.SingleArgument)) await ShowQueue();
        else await AddToQueue();
        Thread.Sleep(1500);
        await ShowCurrentlyPlayingTrack();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }

    private async Task AddToQueue()
    {
        var index =  int.TryParse(Input.SingleArgument, out var idx) ? idx : 0;
        var track = LastSearchedTracks[index];
        await (Client?.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
        WriteSuccessLine($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
    }
}