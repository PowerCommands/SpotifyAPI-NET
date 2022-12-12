using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;
public class AllLocalAlgorithm : TracksAlgorithmBase, IPlaylistAlgorithm
{
    public AllLocalAlgorithm(SpotifyBaseCommando spotifyBaseCommando, int take, bool queue) : base(spotifyBaseCommando) { }
    public async Task<List<PowerCommandTrack>> FindTracksAsync()
    {
        var rand = new Random();
        for (int i = 0; i < BaseCmd.Take; i++)
        {
            var trackIndex = rand.Next(0, Db.Tracks.Count-1);
            var track = Db.Tracks[trackIndex];
            ResultTracks.Add(track);
        }
        return ResultTracks;
    }
}