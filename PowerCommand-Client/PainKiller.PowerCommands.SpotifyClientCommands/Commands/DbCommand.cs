using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: "")]
[PowerCommandDesign( description: "Show diagnostic about your currently stored database",
                         example: "db")]
public class DbCommand : SpotifyBaseCommando
{
    public DbCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        NoClient = true;
        return base.InitializeAndValidateInput(input, designAttribute);
    }

    public override RunResult Run()
    {
        WriteCodeExample("Track count:",$"{SpotifyDB.Tracks.Count}");
        WriteCodeExample("Artist count:",$"{SpotifyDB.Artists.Count}");
        WriteCodeExample("Playlist count:",$"{SpotifyDB.Playlists.Count}");
        return Ok();
    }
}