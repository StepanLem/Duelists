using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneRootBinder : MonoBehaviour
{
    public void LoadScene(SceneFieldReference sceneRef)
    {
        SceneField nextScene = sceneRef.SceneField;
        LoadScene(nextScene);
    }

    public void LoadScene(SceneField nextScene)
    {
        LoadingScreenController.LoadScene(nextScene);
    }
}
