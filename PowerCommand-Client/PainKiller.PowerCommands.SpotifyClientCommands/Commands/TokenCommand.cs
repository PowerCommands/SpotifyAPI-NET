using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Open Spotify Console, you can also save a token, but it is only valid temporary.",
                         options:"save",
                         example: "token")]
public class TokenCommand : CommandBase<PowerCommandsConfiguration>
{
    public TokenCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override RunResult Run()
    {
        if (HasOption("save"))
        {
            var token = DialogService.QuestionAnswerDialog("Token (playlist-read-private and playlist-modify-private):");
            StorageService<Token>.Service.StoreObject(new Token { OathToken = token });
            WriteSuccessLine("Token saved");
        }
        else ShellService.Service.OpenWithDefaultProgram("https://developer.spotify.com/console/get-current-user-playlists/");
        return Ok();
    }
}