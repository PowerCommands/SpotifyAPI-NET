using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Search related artist, using index from an earlier artist search",
                         options: "!related|tracks|append",
                        useAsync: true,
                         example: "artist --related 0|artist --tracks 0")]
public class ArtistCommand : SpotifyBaseCommando
{
    public ArtistCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        if (HasOption("related")) await RelatedArtistsAsync();
        if (HasOption("tracks")) await ArtistsTracks();
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
    private async Task RelatedArtistsAsync()
    {
        var artistIndex = Input.OptionToInt("related");
        var artist = LastSearchedArtists[artistIndex];
        SearchPhrase = $"{artist.Name} related artists";
        var searchResponse = await Client!.Artists.GetRelatedArtists(artist.Id);
        var artists = searchResponse.Artists.Select(a => new PowerCommandArtist(a)).ToList();
        if (artists.Count > 0 && !HasOption("append")) LastSearchedArtists.Clear();
        LastSearchedArtists.AddRange(LastSearchedArtists);
        Print(artists);
    }
    private async Task ArtistsTracks()
    {
        var artistIndex = Input.OptionToInt("tracks");
        var artist = LastSearchedArtists[artistIndex];
        SearchPhrase = $"{artist.Name} top tracks";
        var searchResponse = await Client!.Artists.GetTopTracks(artist.Id, new ArtistsTopTracksRequest("sv"));
        var tracks = searchResponse.Tracks.Select(a => new PowerCommandTrack(a, "search")).ToList();
        if (tracks.Count > 0 && !HasOption("append")) LastSearchedTracks.Clear();
        LastSearchedTracks.AddRange(LastSearchedTracks);
        Print(tracks);
    }
}