using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Turn shuffle on or off",
                       arguments: "!<on or off>",
                        useAsync: true,
                        example: "//Turn shuffle on|shuffle on|//Turn shuffle off|shuffle off")]
public class ShuffleCommand : SpotifyBaseCommando
{
    public ShuffleCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        var shuffleOn = Input.SingleArgument.ToLower() == "on";
        var shuffleStatus = shuffleOn ? "On" : "Off";
        await Client!.Player.SetShuffle(new PlayerShuffleRequest(shuffleOn));
        WriteSuccessLine($"Shuffle is {shuffleStatus}");
        return Ok();
    }
}