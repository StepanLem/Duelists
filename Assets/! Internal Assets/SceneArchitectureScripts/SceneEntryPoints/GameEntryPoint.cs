using System.Collections;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEntryPoint : MonoBehaviour
{
    /// <summary>
    /// ����������� ��� ������ ���� � ����� �� ����. ��� ����������� ���� �� ������ � ���� �������� �� �����.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
        SceneRegistry.InitializeFromScriptableObject();

#if UNITY_EDITOR && TEST
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        //���� ���� ����������� � Bootstrap, �� ������ ������ ���� ��� ���������.
        if (currentSceneBuildIndex == SceneRegistry.BootstrapScene.BuildIndex)
            return;

        //TODO: �������� VR/nonVR ����� � ������� � ����������� �� ������������� �����.
#endif


    }


    private void Awake()
    {
        SceneManager.LoadScene(SceneRegistry.PlayerVRScene.BuildIndex);
        SceneManager.LoadScene(SceneRegistry.MainMenuScene.BuildIndex, LoadSceneMode.Additive);//TODO ������� ��� ����� ����� ��������.
    }
}

