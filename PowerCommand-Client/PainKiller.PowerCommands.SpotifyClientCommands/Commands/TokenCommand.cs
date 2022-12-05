using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

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
            var token = DialogService.QuestionAnswerDialog("Paste your permission token");
            StorageService<Token>.Service.StoreObject(new Token { OathToken = token });
            WriteSuccessLine("Token saved");
        }
        else
        {
            DialogService.QuestionAnswerDialog("You will need this permissions: [playlist-read-private] [playlist-modify-private] [user-read-currently-playing] [user-modify-playback-state] [user-read-playback-state] [user-read-recently-played], hit enter to open the Spotify API page and create a token");
            ShellService.Service.OpenWithDefaultProgram("https://developer.spotify.com/console/post-playlists/");
        }
        return Ok();
    }
}