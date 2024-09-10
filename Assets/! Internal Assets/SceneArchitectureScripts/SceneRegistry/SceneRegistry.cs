using Trisibo;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneRegistry
{
    public static SceneField PlayerVRScene { get; private set; }
    public static SceneField PlayerFlatScene { get; private set; }
    public static SceneField BootstrapScene { get; private set; }
    public static SceneField MainMenuScene { get; private set; }
    public static SceneField LoadingScene {  get; private set; }
    public static SceneField GameplayScene {  get; private set; }



    public static void InitializeFromScriptableObject()
    {
        var initParams = Resources.Load<SceneRegistrySO>("SceneRegistry");

        PlayerVRScene = initParams.PlayerVRScene;
        PlayerFlatScene = initParams.PlayerFlatScene;
        BootstrapScene = initParams.BootstrapScene;
        MainMenuScene = initParams.MainMenuScene;
        LoadingScene = initParams.LoadingScene;
        GameplayScene = initParams.GameplayScene;

        Resources.UnloadAsset(initParams);
    }
}
