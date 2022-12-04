using System.ComponentModel.Design;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Control the Spotify player",
                        useAsync: true,
                        options: "queue",
                         example: "player next")]
public class PlayingCommand : SpotifyBaseCommando
{
    public PlayingCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        await ShowCurrentlyPlayingTrack();
        await ShowQueue();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }

    private async Task ShowQueue()
    {
        var queue = await Client!.Player.GetQueue();
        WriteHeadLine("\nNext in queue:");
        for (var index = 0; index < queue.Queue.Count; index++)
        {
            var playableItem = queue.Queue[index];
            var fullTrack = playableItem as FullTrack;
            if (fullTrack != null) WriteLine($"{index+1} {fullTrack.Artists.FirstOrDefault()?.Name} {fullTrack.Name} {fullTrack.Album.ReleaseDate}");
        }
    }

    private async Task ShowCurrentlyPlayingTrack()
    {
        try
        {
            var response = await (Client?.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track))!).ConfigureAwait(false);
            var fullTrack = response.Item as FullTrack;
            if (fullTrack != null) WriteSuccessLine($"Playing {fullTrack.Artists.FirstOrDefault()?.Name} {fullTrack.Name} {fullTrack.Album.ReleaseDate}");
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
        }
    }
}