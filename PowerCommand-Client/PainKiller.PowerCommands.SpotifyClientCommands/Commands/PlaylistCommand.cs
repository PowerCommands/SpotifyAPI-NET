using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Create playlist, view playlist",
                         options: "!create|from-search|view",
                        useAsync: true,
                         example: "playlist --create <name> --from-search")]
public class PlaylistCommand : SpotifyBaseCommando
{
    public PlaylistCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }

    public override async Task<RunResult> RunAsync()
    {
        if (HasOption("view"))
        {
            Print(SpotifyDB.Playlists);
            return Ok();
        }
        if (Client == null)
        {
            WriteFailure("No client could be loaded, you probably missing a valid token, you need a token with playlist-read-private and playlist-modify-private permissions.");
            var loadSpotifyConsole = DialogService.YesNoDialog("Open the Spotify API console where you could get a new token?");
            if(loadSpotifyConsole) ShellService.Service.OpenWithDefaultProgram("https://developer.spotify.com/console/get-current-user-playlists/");
            return ExceptionError("Spotify API Client is null");
        }
        var tracks = new List<PowerCommandTrack>();
        foreach (var powerCommandTrack in LastSearchedTracks.Where(powerCommandTrack => !tracks.Any(t => t.Name == powerCommandTrack.Name && t.Artist == powerCommandTrack.Artist && t.ReleaseYear == powerCommandTrack.ReleaseYear))) tracks.Add(powerCommandTrack);
        var name = string.IsNullOrEmpty(GetOptionValue("create")) ? "Created by Power commands" : GetOptionValue("create");
        await CreatePlaylist(name, tracks);
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
    public async Task CreatePlaylist(string name, List<PowerCommandTrack> tracks)
    {
        var user = await Client!.UserProfile.Current();
        try
        {
            var playlist = await Client.Playlists.Create($"{user.Id}", new PlaylistCreateRequest(name) { Description = "Demo playlist", Collaborative = false, Public = false });
            var chunkSize = 100;
            var chunkOfTracks = tracks.Take(chunkSize).ToList();
            var chunkCount = 0;
            while (chunkOfTracks.Count > 0 &&  chunkCount < 10)
            {
                var response = await Client.Playlists.AddItems($"{playlist.Id}", new PlaylistAddItemsRequest(chunkOfTracks.Select(l => l.Uri).ToList()));
                WriteLine($"SnapshotId: {response.SnapshotId}");
                foreach (var chunkTrack in chunkOfTracks)
                {
                    var existing = tracks.First(t => t.Id == chunkTrack.Id);
                    tracks.Remove(existing);
                }
                chunkOfTracks.Clear();
                chunkOfTracks.AddRange(tracks.Take(chunkSize));
                chunkCount++;
            }
            
        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
            WriteLine($"{Client.LastResponse!.StatusCode}");
        }
    }
}