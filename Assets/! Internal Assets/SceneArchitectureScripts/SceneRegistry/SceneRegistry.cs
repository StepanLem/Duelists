using Trisibo;
using UnityEngine;

public class SceneRegistry
{
    public static SceneField PlayerVRScene { get; private set; }
    public static SceneField Bootstrap { get; private set; }
    public static SceneField MainMenu { get; private set; }
    public static SceneField Loading {  get; private set; }

    public static void InitializeFromScriptableObject()
    {
        var initParams = Resources.Load<SceneRegistrySO>("SceneRegistry");

        PlayerVRScene = initParams.PlayerVRScene;
        Bootstrap = initParams.Bootstrap;
        MainMenu = initParams.MainMenu;
        Loading = initParams.Loading;

        Resources.UnloadAsset(initParams);
    }
}
