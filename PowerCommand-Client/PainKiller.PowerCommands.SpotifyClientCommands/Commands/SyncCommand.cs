using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;
namespace PainKiller.PowerCommands.SpotifyClientCommands.Commands;

[PowerCommandDesign( description: "Sync current users playlist",
                         options: "name",
                        useAsync: true,
                         example: "sync")]
public class SyncCommand : SpotifyBaseCommando
{
    private readonly List<PowerCommandPlaylist> _playlists = new();
    public SyncCommand(string identifier, PowerCommandsConfiguration configuration) : base(identifier, configuration) { }
    public override async Task<RunResult> RunAsync()
    {
        var userPlaylistsPage = await Client?.Playlists.CurrentUsers()!;
        var playlists = await EnumeratePlaylistsAsync(userPlaylistsPage);

        foreach (var noTracksLoadedPlayList in playlists)
        {
            var pcPlaylist = new PowerCommandPlaylist(noTracksLoadedPlayList);
            var playlist = await Client.Playlists.Get(noTracksLoadedPlayList.Id);
            WriteHeadLine($"{playlist.Name}");
            if (playlist.Tracks != null)
            {
                await EnumeratePlaylistTracksAsync(playlist.Tracks, pcPlaylist);
            }
            _playlists.Add(pcPlaylist);
        }
        SpotifyDB.Playlists.AddRange(_playlists);
        SpotifyDB.Updated = DateTime.Now;
        SyncTags();
        StorageService<SpotifyDB>.Service.StoreObject(SpotifyDB);
        WriteSuccessLine("The playlist are now downloaded and stored locally, earlier stored tags (if any) are restored.");
        Write(ConfigurationGlobals.Prompt);
        return Ok();
    }
    private async Task<List<SimplePlaylist>> EnumeratePlaylistsAsync(Paging<SimplePlaylist> page)
    {
        var retVal = new List<SimplePlaylist>();
        await foreach (var item in Client!.Paginate(page))
        {
            WriteLine(item.Name);
            retVal.Add(item);
        }
        return retVal;
    }
    private async Task EnumeratePlaylistTracksAsync(Paging<PlaylistTrack<IPlayableItem>> page, PowerCommandPlaylist pcPlaylist)
    {
        await foreach (var item in Client!.Paginate(page))
            if (item.Track is FullTrack track)
            {
                Console.WriteLine($"{track.Artists.First().Name} {track.Name}");
                var pcTrack = new PowerCommandTrack(track, pcPlaylist.Name);
                pcPlaylist.Tracks.Add(pcTrack);
                if (SpotifyDB.Tracks.All(t => t.Id != track.Id))
                {
                    SpotifyDB.Tracks.Add(pcTrack);
                }
            }
    }

    private void SyncTags()
    {
        var taggedTracks = StorageService<TaggedTracks>.Service.GetObject();
        foreach (var powerCommandTrack in SpotifyDB.Tracks.Where(powerCommandTrack => taggedTracks.Tracks.Any(t => t.Id == powerCommandTrack.Id))) powerCommandTrack.Tags = taggedTracks.Tracks.First(t => t.Id == powerCommandTrack.Id).Tags;
    }
}