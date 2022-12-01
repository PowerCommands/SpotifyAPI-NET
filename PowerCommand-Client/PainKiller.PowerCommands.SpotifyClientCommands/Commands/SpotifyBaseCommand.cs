using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;
public abstract class SpotifyBaseCommando : CommandBase<PowerCommandsConfiguration>
{
    protected static bool NoClient;
    protected static SpotifyDB SpotifyDB = new();
    protected SpotifyClient? Client;
    protected SpotifyBaseCommando(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        SpotifyDB = StorageService<SpotifyDB?>.Service.GetObject() ?? new SpotifyDB();
        if (!NoClient)
        {
            var token = DialogService.QuestionAnswerDialog("Access token (just hit enter to use current saved one): ");
            if (!string.IsNullOrEmpty(token)) StorageService<Token>.Service.StoreObject(new Token { OathToken = token });
            else token = StorageService<Token>.Service.GetObject().OathToken;
            Client = new SpotifyClient($"{token}");
        }
        return base.InitializeAndValidateInput(input, designAttribute);
    }
}