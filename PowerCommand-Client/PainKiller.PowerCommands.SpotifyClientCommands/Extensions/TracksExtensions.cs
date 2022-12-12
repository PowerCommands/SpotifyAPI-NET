using PainKiller.PowerCommands.SpotifyClientCommands.Commands;
using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Extensions;

public static class TracksExtensions
{
    public static async Task AddToQueueOrJustShowResult(this List<PowerCommandTrack> tracks, SpotifyBaseCommando commando)
    {
        if (commando.AddToQueue) foreach (var powerCommandTrack in tracks) await commando.AddTracksToQueue(powerCommandTrack);
        else commando.Print(tracks);
    }
}