using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }
}
