using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign(description: "Open Spotify Console, you can also save a token, but it is only valid temporary.",
    options: "save",
    example: "token")]
public class TokenCommand : CommandBase<PowerCommandsConfiguration>
{
    public TokenCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override RunResult Run()
    {
        ShellService.Service.OpenWithDefaultProgram("https://developer.spotify.com/console/post-playlists/");
        var token = DialogService.QuestionAnswerDialog("Paste your permission token");
        StorageService<Token>.Service.StoreObject(new Token { OathToken = token });
        WriteSuccessLine("Token saved");
        return Ok();
    }
}