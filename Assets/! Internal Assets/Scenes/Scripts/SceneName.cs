public static class SceneName
{
    public const string Bootstrap = "Bootstrap";
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";
    public const string Loading = "LoadingScene";

    public static bool IsCorrectName(string name)
    {
        return name == Bootstrap ||
               name == MainMenu ||
               name == Gameplay || 
               name == Loading;
    }
}
