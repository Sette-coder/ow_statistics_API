namespace ow_backendAPI.Constants;

public static class MapMode
{
    public const string Hybrid     = "Hybrid";
    public const string Escort     = "Escort";
    public const string Control    = "Control";
    public const string Push       = "Push";
    public const string Flashpoint = "Flashpoint";

    // ModeId used for grouping/filtering on the client side
    public static int GetModeId(string mode) => mode switch
    {
        Hybrid     => 1,
        Escort     => 2,
        Control    => 3,
        Push       => 4,
        Flashpoint => 5,
        _          => 0
    };
}