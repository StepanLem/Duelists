using UnityEngine;

public class CanvasRenderModeSwitcher : MonoBehaviour, IGameModeSwitchable
{
    [SerializeField] private Canvas _canvas;

    private void Awake()
    {
        if (FlatOrXRGameModeManager.IsXRModeActive)
            InitializeForXRMode();
        else
            InitializeForFlatMode();
    }

    public void InitializeForXRMode()
    {
        _canvas.renderMode = RenderMode.WorldSpace;
    }

    public void InitializeForFlatMode()
    {
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

  
}