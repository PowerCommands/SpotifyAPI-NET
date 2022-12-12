using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

public class RelatedSearchAlgorithm : TracksAlgorithmBase, IPlaylistAlgorithm
{
    public RelatedSearchAlgorithm(SpotifyBaseCommando spotifyBaseCommando) : base(spotifyBaseCommando)
    {
        
    }
    public async Task<List<PowerCommandTrack>> FindTracksAsync()
    {
        var rand = new Random();
        var tracks = new List<PowerCommandTrack>();
        tracks.AddRange(SpotifyBaseCommando.LastSearchedTracks);
        SpotifyBaseCommando.LastSearchedTracks.Clear();
        var artistsIds = tracks.Select(t => rand.Next(0, tracks.Count - 1)).Select(trackIndex => tracks[trackIndex].ArtistId).ToList();

        var related = new List<PowerCommandTrack>();
        foreach (var artist in artistsIds)
        {
            try
            {
                var relatedArtistResponse = await Client.Artists.GetRelatedArtists(artist);
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
        return related;
    }
}