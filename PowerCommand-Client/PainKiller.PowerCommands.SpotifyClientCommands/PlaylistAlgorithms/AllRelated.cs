using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

public class AllRelated : TracksAlgorithmBase, IPlaylistAlgorithm
{
    private readonly int _take;
    private readonly bool _queue;

    public AllRelated(SpotifyBaseCommando spotifyBaseCommando, int take, bool queue) : base(spotifyBaseCommando)
    {
        _take = take;
        _queue = queue;
    }
    public async Task RunAsync()
    {
        var rand = new Random();
        var artists = new List<PowerCommandArtist>();
        for (int i = 0; i < _take; i++)
        {
            var trackIndex = rand.Next(0, Db.Artists.Count - 1);
            var artist = Db.Artists[trackIndex];
            artists.Add(artist);
        }
        Writer.WriteLine("Artists found");
        var related = new List<PowerCommandTrack>();
        foreach (var artist in artists)
        {
            try
            {
                var relatedArtistResponse = await Client.Artists.GetRelatedArtists(artist.Id);
                var artistIndex = rand.Next(0, relatedArtistResponse.Artists.Count - 1);
                if (relatedArtistResponse.Artists.Count == 0) continue;
                var reletedArtist = relatedArtistResponse.Artists[artistIndex];
                var reletaedTracks = await Client.Artists.GetTopTracks(reletedArtist.Id, new ArtistsTopTracksRequest("sv"));
                var trackIndex = rand.Next(0, reletaedTracks.Tracks.Count - 1);
                if (reletaedTracks.Tracks.Count == 0) continue;
                var relatedTrack = reletaedTracks.Tracks[trackIndex];
                related.Add(new PowerCommandTrack(relatedTrack, "random"));
            }
            catch{ Writer.WriteFailure("No related artist or related track found");}
        }
        BaseCmd.Print(related);
        if (_queue) await AddTracksToQueue(-1);
    }
}