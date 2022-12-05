using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Tag your music using the latest search result",
                         options: "remove",
                         example: "tag")]
public class TagCommand : SpotifyBaseCommando
{
    private TaggedTracks _tagged = new();
    public TagCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override bool InitializeAndValidateInput(ICommandLineInput input, PowerCommandDesignAttribute? designAttribute = null)
    {
        NoClient = true;
        return base.InitializeAndValidateInput(input, designAttribute);
    }
    public override RunResult Run()
    {
        var tag = string.IsNullOrEmpty(Input.SingleArgument) ? Input.SingleQuote : Input.SingleArgument;
        if (LastSearchedTracks.Count == 0) return BadParameterError("You must have a search result to tag");
        if(HasOption("remove")) RemoveTagToTracks(tag);
        else AddTagToTracks(tag);
        return Ok();
    }
    private void AddTagToTracks(string tag)
    {
        _tagged = StorageService<TaggedTracks>.Service.GetObject();
        foreach (var track in LastSearchedTracks)
        {
            var existing = SpotifyDB.Tracks.FirstOrDefault(t => t.Id == track.Id);
            if (existing != null && !existing.Tags.Contains($"#{tag}")) existing.Tags = $"{existing.Tags}#{tag}";
            if(!track.Tags.Contains($"#{tag}")) track.Tags = $"{track.Tags}#{tag}";
        }
        Print(LastSearchedTracks);
        var saveChanges = DialogService.YesNoDialog("Do you want to save tracks with the added tag?");
        if (!saveChanges) return;
        
        var unMofidiedTracks = new List<PowerCommandTrack>();
        foreach (var unModifiedTrack in _tagged.Tracks)
        {
            var existing = LastSearchedTracks.FirstOrDefault(t => t.Id == unModifiedTrack.Id);
            if (existing != null) continue;
            unMofidiedTracks.Add(unModifiedTrack);
        }
        unMofidiedTracks.AddRange(LastSearchedTracks);
        _tagged.Tracks.Clear();
        _tagged.Tracks.AddRange(unMofidiedTracks);
        StorageService<SpotifyDB>.Service.StoreObject(SpotifyDB);
        StorageService<TaggedTracks>.Service.StoreObject(_tagged);
        WriteSuccessLine("The tracks has been saved to file.");
    }

    private void RemoveTagToTracks(string tag)
    {
        _tagged = StorageService<TaggedTracks>.Service.GetObject();
        foreach (var track in LastSearchedTracks)
        {
            var existing = SpotifyDB.Tracks.FirstOrDefault(t => t.Id == track.Id);
            if (existing != null && existing.Tags.Contains($"#{tag}")) existing.Tags = $"{existing.Tags}#{tag}".Replace($"#{tag}", "");
            if (track.Tags.Contains($"#{tag}")) track.Tags = track.Tags.Replace($"#{tag}", "");
        }
        Print(LastSearchedTracks);
        var saveChanges = DialogService.YesNoDialog("Do you want to save tracks with the removed tag?");
        if (!saveChanges) return;
        
        var unMofidiedTracks = new List<PowerCommandTrack>();
        foreach (var unModifiedTrack in _tagged.Tracks)
        {
            var existing = LastSearchedTracks.FirstOrDefault(t => t.Id == unModifiedTrack.Id);
            if (existing != null) if (existing.Tags.Contains($"#{tag}")) existing.Tags = existing.Tags.Replace($"#{tag}", "");
            unMofidiedTracks.Add(unModifiedTrack);
        }
        unMofidiedTracks.AddRange(LastSearchedTracks);
        _tagged.Tracks.Clear();
        _tagged.Tracks.AddRange(unMofidiedTracks);
        StorageService<SpotifyDB>.Service.StoreObject(SpotifyDB);
        StorageService<TaggedTracks>.Service.StoreObject(_tagged);
        WriteSuccessLine("The tracks has been saved to file.");
    }
}