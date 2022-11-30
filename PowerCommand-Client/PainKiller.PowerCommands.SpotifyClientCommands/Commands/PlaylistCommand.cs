using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandTest(         tests: " ")]
[PowerCommandDesign( description: "View current users playlist",
                         options: "name",
                        useAsync: true,
                         example: "playlist --name \"playlistname\"")]
public class PlaylistCommand : CommandBase<PowerCommandsConfiguration>
{
    private SpotifyClient? _client;
    public PlaylistCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        var token = DialogService.QuestionAnswerDialog("Access token: ");
        _client = new SpotifyClient(token);
        var userPlaylistsPage = await _client.Playlists.CurrentUsers();
        var playlists = await EnumeratePlaylistsAsync(userPlaylistsPage);
        var findEmptyPlayList = playlists.FirstOrDefault(p => p.Name.ToLower().Contains(GetOptionValue("name")));
        if (findEmptyPlayList != null)
        {
            var playlist = await _client.Playlists.Get(findEmptyPlayList.Id);
            WriteHeadLine($"{playlist.Name}");
            if (playlist.Tracks != null) await EnumeratePlaylistTracksAsync(playlist.Tracks);
        }
        return Ok();
    }
    private async Task<List<SimplePlaylist>> EnumeratePlaylistsAsync(Paging<SimplePlaylist> page)
    {
        var retVal = new List<SimplePlaylist>();
        await foreach (var item in _client!.Paginate(page))
        {
            //WriteLine(item.Name);
            retVal.Add(item);
        }
        return retVal;
    }
    private async Task EnumeratePlaylistTracksAsync(Paging<PlaylistTrack<IPlayableItem>> page)
    {
        await foreach (var item in _client!.Paginate(page)) if (item.Track is FullTrack track) Console.WriteLine($"{track.Artists.First().Name} {track.Name} {track.Album.Name}");
    }
}