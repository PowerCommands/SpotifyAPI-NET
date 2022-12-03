using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "View all tags",
                         options: "!view-group-by-artist",
                         example: "tags")]
public class TagsCommand : SpotifyBaseCommando
{
    private TaggedTracks _taggedTracks= new();
    public TagsCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        NoClient = true;
        return base.InitializeAndValidateInput(input, designAttribute);
    }
    public override RunResult Run()
    {
        _taggedTracks = StorageService<TaggedTracks>.Service.GetObject();
        if (HasOption("view-group-by-artist"))
        {
            View(GetOptionValue("view-group-by-artist"));
            return Ok();
        }
        foreach (var tagName in _taggedTracks.Tracks.Select(t => t.Tags).Distinct()) WriteLine(tagName);
        //insert your code here
        return Ok();
    }
    private void View(string tag)
    {
        var taggedTracks = SearchTag(tag).Select(t => t.Artist).Distinct();
        foreach (var artist in taggedTracks) WriteLine(artist);
    }
}