namespace PainKiller.PowerCommands.SpotifyClientCommands.Extensions;

public static class FormatExtensions
{
    public static string BuildSearchPhrase(this ICommandLineInput input)
    {
        var track = input.HasOption("track") ?  $" track:\"{input.GetOptionValue("track")}\"" : "";
        var genre = input.HasOption("genre") ? $" genre:\"{input.GetOptionValue("genre")}\"" : "";
        var album = input.HasOption("album") ?  $" album:\"{input.GetOptionValue("album")}\"" : "";
        var artist = input.HasOption("artist") ? $" artist:\"{input.GetOptionValue("artist")}\"" : "";
        var year = input.HasOption("year") ?  $" year:\"{input.GetOptionValue("year")}\"" : "";

        var search = $"{genre}{artist}{track}{album}{year}";
        return search;
    }
}