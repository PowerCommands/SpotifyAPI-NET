using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Keep current token alive, runs until error occurs or 10 hours elapsed.",
                        useAsync: true,
                         example: "heartbeat")]
public class HeartbeatCommand : CommandBase<PowerCommandsConfiguration>
{
    public HeartbeatCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        var maxReRuns = 6000;   //10 hour should be enough
        var iteration = 0;
        var autorun = true;
        var token = StorageService<Token>.Service.GetObject().OathToken;
        var client = new SpotifyClient($"{token}");
        while (autorun)
        {
            iteration++;
            if (iteration > maxReRuns) autorun = false;
            try
            {
                var user = await client.UserProfile.Current();
                OverwritePreviousLine($"Current user: {user.DisplayName}");
                WriteLine("Idle...");
                Thread.Sleep(60000);
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
                WriteLine($"{client.LastResponse!.StatusCode}");
            }
        }
        return Quit();
    }
}