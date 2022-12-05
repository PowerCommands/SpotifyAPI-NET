using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Control the Spotify player",
                        useAsync: true,
                        options: "queue|history",
                         example: "player next")]
public class PlayerCommand : SpotifyBaseCommando
{
    public PlayerCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        var action = Input.SingleArgument.ToLower();
        switch (action)
        {
            case "next":
                await (Client?.Player.SkipNext()!).ConfigureAwait(false);
                WriteSuccessLine("player next");
                break;
            case "back":
            case "previous":
                await (Client?.Player.SkipPrevious()!).ConfigureAwait(false);
                WriteSuccessLine("player back");
                break;
            case "pause":
            case "stop":
            case "break":
                await (Client?.Player.PausePlayback()!).ConfigureAwait(false);
                WriteSuccessLine("break");
                break;
            case "play":
            case "resume":
                await (Client?.Player.ResumePlayback()!).ConfigureAwait(false);
                WriteSuccessLine("player resume");
                break;
            default:
                if (HasOption("queue")) await AddToQueue();
                if (HasOption("history")) await ShowRecentPlayedTrack();
                break;
        }
        Thread.Sleep(1500);
        await ShowCurrentlyPlayingTrack();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }

    private async Task AddToQueue()
    {
        var index =  Input.OptionToInt("queue");
        var track = LastSearchedTracks[index];
        await (Client?.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
        WriteSuccessLine($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
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
    private async Task ShowRecentPlayedTrack()
    {
        try
        {
            LastSearchedTracks.Clear();
            var response = await Client?.Player.GetRecentlyPlayed()!;
            if (response.Items != null)
                foreach (var item in response.Items)
                    LastSearchedTracks.Add(new PowerCommandTrack(item.Track, "recently played"));
            Print(LastSearchedTracks);
        }
        catch (Exception ex)
        {
            WriteLine(ex.Message);
        }
    }
}