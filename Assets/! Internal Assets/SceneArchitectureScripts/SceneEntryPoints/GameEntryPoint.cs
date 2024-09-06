using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{
    //����������� ��� ������ ���� � ����� �� ����. � �� ����������� ���� �� ������ � ���� �������� �� �����.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
        SceneRegistry.InitializeFromScriptableObject();

#if UNITY_EDITOR && TEST
        if (SceneRegistry.Bootstrap.BuildIndex == SceneManager.GetActiveScene().buildIndex) //���� ���� ��������� � Bootstrap, �� ��������� ������� ������
            return;

        
        LoadImportantScenes();
#endif
    }

    private static void LoadImportantScenes()
    {
        var isBootstrapLoaded = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (scene.buildIndex == SceneRegistry.Bootstrap.BuildIndex)
                isBootstrapLoaded = true;

            //TODO: �� �� ����� �� ������ ������. � ����������� �� ����, ��������� �� ����.
        }

        if (!isBootstrapLoaded)
            SceneManager.LoadSceneAsync(SceneRegistry.Bootstrap.BuildIndex, LoadSceneMode.Additive);
    }
}

