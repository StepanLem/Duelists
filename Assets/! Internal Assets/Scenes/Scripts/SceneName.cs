public static class SceneName
{
    public const string BOOTSTRAP = "Bootstrap";
    public const string MAIN_MENU = "MainMenu";
    public const string GAMEPLAY = "Gameplay";

    public static bool IsCorrectName(string name)
    {
        return name == BOOTSTRAP ||
               name == MAIN_MENU ||
               name == GAMEPLAY;
    }
}
