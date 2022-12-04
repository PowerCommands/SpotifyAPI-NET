using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "View spotify genres (or category as they calling it in their API model",
                         options: "overwrite",
                        useAsync: true,
                         example: "genre")]
public class GenreCommand : SpotifyBaseCommando
{
    public GenreCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        if (SpotifyDB.Playlists.Count == 0) SpotifyDB = StorageService<SpotifyDB>.Service.GetObject();
        if (SpotifyDB.Genres.Count > 0 && !HasOption("overwrite"))
        {
            NoClient = true;
            return false;
        }
        return base.InitializeAndValidateInput(input, designAttribute);
    }
    public override async Task<RunResult> RunAsync()
    {
        if (SpotifyDB.Genres.Count == 0 || HasOption("overwrite"))
        {
            var searchResponse = await Client!.Browse.GetRecommendationGenres();
            foreach (var genre in searchResponse.Genres) WriteLine(genre);
            SpotifyDB.Genres.Clear();
            SpotifyDB.Genres.AddRange(searchResponse.Genres);
            StorageService<SpotifyDB>.Service.StoreObject(SpotifyDB);
            Write(ConfigurationGlobals.Prompt);
            return Ok();
        }
        foreach (var genre in SpotifyDB.Genres) WriteLine(genre);
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
}