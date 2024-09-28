using System;

public static class GameModeManager
{
    public static bool IsXRModeActive { get; private set; }

    public static event Action OnXRModeActivated;

    public static bool TryActivateXRMode()
    {
        if (XRDisplayDetectionUtility.IsSubsystemConnected())
        {
            ActivateXRMode();
            return true;
        }
        else return false;
    }

    private static void ActivateXRMode()
    {
        IsXRModeActive = true;
        OnXRModeActivated?.Invoke();
    }
}
