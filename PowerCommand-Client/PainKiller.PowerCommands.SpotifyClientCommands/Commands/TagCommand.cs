using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Tag your music using the latest search result",
                         options: "!create|!view-group-by-artist",
                         example: "tag")]
public class TagCommand : SpotifyBaseCommando
{
    private TaggedTracks _taggedTracks= new();
    public TagCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        NoClient = true;
        return base.InitializeAndValidateInput(input, designAttribute);
    }

    public override RunResult Run()
    {
        if (HasOption("view-group-by-artist"))
        {
            View(GetOptionValue("view-group-by-artist"));
            return Ok();
        }
        var tag = GetOptionValue("create");
        _taggedTracks = StorageService<TaggedTracks>.Service.GetObject();
        foreach (var track in LastSearch)
        {
            var existing = SpotifyDB.Tracks.FirstOrDefault(t => t.Id == track.Id);
            if (existing != null && !existing.Tags.Contains($"#{tag}")) existing.Tags = $"{existing.Tags}#{tag}";
            if(!track.Tags.Contains($"#{tag}")) track.Tags = $"{track.Tags}#{tag}";
        }
        Print(LastSearch);
        var saveChanges = DialogService.YesNoDialog("Do you want to save tracks with the added tag?");
        if (!saveChanges) return Ok();
        
        var unMofidiedTracks = new List<PowerCommandTrack>();
        foreach (var unModifiedTrack in _taggedTracks.Tracks)
        {
            var existing = LastSearch.FirstOrDefault(t => t.Id == unModifiedTrack.Id);
            if (existing != null) continue;
            unMofidiedTracks.Add(unModifiedTrack);
        }
        unMofidiedTracks.AddRange(LastSearch);
        _taggedTracks.Tracks.Clear();
        _taggedTracks.Tracks.AddRange(unMofidiedTracks);
        StorageService<SpotifyDB>.Service.StoreObject(SpotifyDB);
        StorageService<TaggedTracks>.Service.StoreObject(_taggedTracks);
        WriteSuccessLine("The tracks has been saved to file.");
        return Ok();
    }

    public void View(string tag)
    {
        var taggedTracks = SearchTag(tag).Select(t => t.Artist).Distinct();
        foreach (var artist in taggedTracks)
        {
            WriteLine(artist);
        }
    }
}