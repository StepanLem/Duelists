using UnityEngine;

public class GameplayExitGameButton : MonoBehaviour
{
    public void ExitGameplay()
    {
        LoadingScreenController.LoadScene(SceneRegistry.MainMenuScene);
    }
}
