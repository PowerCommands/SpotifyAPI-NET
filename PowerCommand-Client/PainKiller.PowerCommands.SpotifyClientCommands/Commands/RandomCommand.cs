using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Random a track list, default random count is 100 tracks",
                        useAsync: true,
                         options: "!count|queue",
                         example: "Random 100 tracks from your play-lists|random|//Random 50 tracks|random --count 50|//Random 100 track and add them to queue|random --queue")]
public class RandomCommand : QueueCommand
{
    public RandomCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        var take = Input.OptionToInt("count");
        take = take == 0 ? 100 : take;
        LastSearchedTracks.Clear();
        var rand = new Random();
        var tracks = new List<PowerCommandTrack>();
        for (int i = 0; i < take; i++)
        {
            var trackIndex = rand.Next(0, SpotifyDB.Tracks.Count -1);
            var track = SpotifyDB.Tracks[trackIndex];
            tracks.Add(track);
        }
        Print(tracks);
        if (HasOption("queue")) await AddTracksToQueue();
        return Ok();
    }
}