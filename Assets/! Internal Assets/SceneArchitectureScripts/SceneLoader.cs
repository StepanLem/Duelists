using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ДОЛЖЕН БЫТЬ В ОТДЕЛЬНОМ GAMEOBJECT В КОРНЕ СЦЕНЫ
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        if (transform.parent)
        {
            Debug.LogError("SceneLoader ДОЛЖЕН БЫТЬ В ОТДЕЛЬНОМ GAMEOBJECT В КОРНЕ СЦЕНЫ");
        }
    }

    public void LoadScene(SceneFieldReference sceneFieldRef)
    {
        StartCoroutine(LoadSceneRoutine(sceneFieldRef));
    }

    private IEnumerator LoadSceneRoutine(SceneFieldReference sceneFieldRef)
    {
        DontDestroyOnLoad(gameObject);

        AsyncOperation sceneLoading;
        AsyncOperation unloadScene;

        sceneLoading = SceneManager.LoadSceneAsync(SceneRegistry.Loading.BuildIndex, LoadSceneMode.Additive);
        yield return new WaitUntil(() => sceneLoading.isDone);
        Image loadingImage = FindObjectOfType<LoadingScene>().LoadingImage;

        unloadScene = SceneManager.UnloadSceneAsync(GameEntryPoint.CurrentSceneBuildIndex);
        yield return new WaitUntil(() => unloadScene.isDone);

        int sceneFieldBuildIndex = sceneFieldRef.SceneField.BuildIndex;
        sceneLoading = SceneManager.LoadSceneAsync(sceneFieldBuildIndex, LoadSceneMode.Additive);
        while (!sceneLoading.isDone)
        {
            loadingImage.fillAmount = sceneLoading.progress;
#if UNITY_EDITOR
            if (sceneLoading.progress > .5f)
            {
                yield return new WaitForSeconds(1f);
            }
#endif
            yield return null;
        }

        unloadScene = SceneManager.UnloadSceneAsync(SceneRegistry.Loading.BuildIndex);
        yield return new WaitUntil(() => unloadScene.isDone);

        GameEntryPoint.CurrentSceneBuildIndex = sceneFieldBuildIndex;

        Destroy(gameObject);
    }
}
