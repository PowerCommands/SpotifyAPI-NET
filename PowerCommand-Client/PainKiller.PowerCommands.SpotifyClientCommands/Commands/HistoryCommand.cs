namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Show recent played tracks",
                        useAsync: true,
                        example: "history")]
public class HistoryCommand : PlayerCommand
{
    public HistoryCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        await ShowRecentPlayedTrack();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
    
}