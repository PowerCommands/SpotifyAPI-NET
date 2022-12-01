using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "Get current user if your token is valid",
                        useAsync: true,
                         example: "user")]
public class UserCommand : SpotifyBaseCommando
{
    public UserCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        if (Client == null)
        {
            WriteFailure("No client could be loaded, you probably missing a valid token, you need a token with playlist-read-private and playlist-modify-private permissions.");
            var loadSpotifyConsole = DialogService.YesNoDialog("Open the Spotify API console where you could get a new token?");
            ShellService.Service.OpenWithDefaultProgram("https://developer.spotify.com/console/get-current-user-playlists/");
            return ExceptionError("Spotify API Client is null");
        }
        try
        {
            var user = await Client.UserProfile.Current();
            WriteSuccessLine($"Current user: {user.DisplayName}");
            Write(ConfigurationGlobals.Prompt);
            return Ok();
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
            WriteLine($"{Client.LastResponse!.StatusCode}");
        }

        return ExceptionError("Exception when trying to use the Spotify API, Token outdated probably");
    }
}