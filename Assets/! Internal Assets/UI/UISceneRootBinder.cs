using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneRootBinder : MonoBehaviour
{
    public void HandleStartButtonClick(SceneFieldReference sceneRef)
    {
        SceneField nextScene = sceneRef.SceneField;
        LoadingScreenController.AddSceneToLoadOnNextLoadingScreen(nextScene);
        LoadingScreenController.AddSceneToUnloadOnNextLoadingScreen(SceneManager.GetActiveScene().buildIndex);
        LoadingScreenController.InvokeLoadingScreen(nextScene);
    }
}
