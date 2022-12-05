using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

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
        var tracks = new List<PowerCommandTrack>();
        if(index > -1) tracks.Add(LastSearchedTracks[index]);
        else tracks.AddRange(LastSearchedTracks);
        foreach (var track in tracks)
        {
            await (Client?.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
            WriteSuccessLine($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
        }
    }
}