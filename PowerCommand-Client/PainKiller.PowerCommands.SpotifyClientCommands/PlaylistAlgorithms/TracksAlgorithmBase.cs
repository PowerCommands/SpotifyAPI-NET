using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

public abstract class TracksAlgorithmBase
{
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
        var tracks = new List<PowerCommandTrack>();
        if(index > -1) tracks.Add(SpotifyBaseCommando.LastSearchedTracks[index]);
        else tracks.AddRange(SpotifyBaseCommando.LastSearchedTracks);
        foreach (var track in tracks)
        {
            await (BaseCmd.Client!.Player.AddToQueue(new PlayerAddToQueueRequest(track.Uri))!).ConfigureAwait(false);
            Writer.WriteSuccess($"player queued track {track.Artist} {track.Name} released: {track.ReleaseYear}");
        }
    }
}