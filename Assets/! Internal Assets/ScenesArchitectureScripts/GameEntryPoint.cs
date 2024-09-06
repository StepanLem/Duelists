using R3;
using System.Threading.Tasks;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint : MonoBehaviour
{

    [SerializeField] private SceneField _playerVRScene;
    [SerializeField] private SceneField _menuScene;


    //����������� ��� ������ ���� � ����� �� ����. � �� ����������� ���� �� ������ � ���� �������� �� �����.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeGameStart()
    {
#if UNITY_EDITOR && TEST
        if(SceneManager.GetSceneByName(SceneNames.Bootstrap) != SceneManager.GetActiveScene()) //���� ���� ��������� � Bootstrap, �� ��������� ������� ������
            LoadImportantScenes();
#endif
    }

    private static void LoadImportantScenes()
    {
        var isBootstrapLoaded = false;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {


            var scene = SceneManager.GetSceneAt(i);

            //�� ����� ����� ������� ������� ����� ScriptableObject
            //�� ����� ������� � ���� ������������ � ������� sceneField.
            //����� �� ��������� ��������� ������� + �������� � ���������� ���������� �� �����.
            //������ � ��� ��������� ��������� �������� ������ �� SO.
            //TODO: ���� ����������� � ������.
            if (scene.name == SceneNames.Bootstrap)
                isBootstrapLoaded = true;

            //TODO: �� �� ����� �� ������ ������. � ����������� �� ����, ��������� �� ����.

        }

        if (!isBootstrapLoaded)
            SceneManager.LoadSceneAsync(SceneNames.Bootstrap, LoadSceneMode.Additive);
    }
}

