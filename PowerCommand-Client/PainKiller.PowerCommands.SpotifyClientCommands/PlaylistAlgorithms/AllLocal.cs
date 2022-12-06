using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;
public class AllLocal : TracksAlgorithmBase, IPlaylistAlgorithm
{
    private readonly int _take;
    private readonly bool _queue;

    public AllLocal(SpotifyBaseCommando spotifyBaseCommando, int take, bool queue) : base(spotifyBaseCommando)
    {
        _take = take;
        _queue = queue;
    }
    public async Task RunAsync()
    {
        var rand = new Random();
        var tracks = new List<PowerCommandTrack>();
        for (int i = 0; i < _take; i++)
        {
            var trackIndex = rand.Next(0, Db.Tracks.Count-1);
            var track = Db.Tracks[trackIndex];
            tracks.Add(track);
        }
        BaseCmd.Print(tracks);
        if (_queue) await AddTracksToQueue(-1);
    }
}