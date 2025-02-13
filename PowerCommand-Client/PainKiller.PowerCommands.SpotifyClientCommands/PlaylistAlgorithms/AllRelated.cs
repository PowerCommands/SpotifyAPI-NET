﻿using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;
using SpotifyAPI.Web;

namespace PainKiller.PowerCommands.SpotifyClientCommands.PlaylistAlgorithms;

public class AllRelatedAlgorithm : TracksAlgorithmBase, IPlaylistAlgorithm
{
    private readonly int _take;
    private readonly bool _queue;

    public AllRelatedAlgorithm(SpotifyBaseCommando spotifyBaseCommando, int take, bool queue) : base(spotifyBaseCommando)
    {
        _take = take;
        _queue = queue;
    }
    public async Task<List<PowerCommandTrack>> FindTracksAsync()
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
                ResultTracks.Add(new PowerCommandTrack(relatedTrack, "random"));
            }
            catch{ Writer.WriteFailure("No related artist or related track found");}
        }
        if (_queue) await AddTracksToQueue(-1);
        return ResultTracks;
    }
}