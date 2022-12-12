using PainKiller.PowerCommands.SpotifyClientCommands.DomainObjects;

namespace PainKiller.PowerCommands.SpotifyClientCommands.Contracts;
public interface IPlaylistAlgorithm
{
    Task<List<PowerCommandTrack>> FindTracksAsync();
}