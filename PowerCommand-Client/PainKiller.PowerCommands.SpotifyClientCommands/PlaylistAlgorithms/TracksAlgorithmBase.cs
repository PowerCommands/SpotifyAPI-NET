using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

public abstract class TracksAlgorithmBase
{
    protected List<PowerCommandTrack> ResultTracks = new();
    protected SpotifyBaseCommando BaseCmd { get; }
    protected IConsoleWriter Writer { get; }
    protected SpotifyClient Client => BaseCmd.Client!;
    protected SpotifyDB Db => SpotifyBaseCommando.SpotifyDB;
    protected TracksAlgorithmBase(SpotifyBaseCommando spotifyBaseCommando)
    {
        BaseCmd = spotifyBaseCommando;
        Writer = BaseCmd;
    }
    protected async Task AddTracksToQueue(int index)
    {
        if(index > -1)
        {
            var track = ResultTracks[index];
            await (BaseCmd.Client!.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
            Writer.WriteSuccess($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
        }
        else
        {
            foreach (var track in ResultTracks)
            {
                await (BaseCmd.Client!.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
                Writer.WriteSuccess($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
            }
        }
    }
}