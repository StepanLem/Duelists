using System;
using Trisibo;
using UnityEngine;

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

    public static SceneField GetSceneFieldByBuildIndex(int buildIndex)
    {
        if (PlayerVRScene.BuildIndex == buildIndex)
        {
            return PlayerVRScene;
        }
        if (PlayerFlatScene.BuildIndex == buildIndex)
        {
            return PlayerFlatScene;
        }
        if (BootstrapScene.BuildIndex == buildIndex)
        {
            return BootstrapScene;
        }
        if (MainMenuScene.BuildIndex == buildIndex)
        {
            return MainMenuScene;
        }
        if (LoadingScene.BuildIndex == buildIndex)
        {
            return LoadingScene;
        }
        if (GameplayScene.BuildIndex == buildIndex)
        {
            return GameplayScene;
        }
        throw new NotImplementedException();
    }
}
