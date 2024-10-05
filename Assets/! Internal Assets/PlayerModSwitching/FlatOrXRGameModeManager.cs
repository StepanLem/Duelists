using System;

public static class FlatOrXRGameModeManager
{
    public static bool IsXRModeActive { get; private set; }

    public static event Action OnXRModeActivated;

    public static bool TryActivateXRMode()
    {
        if (!XRDisplayDetectionUtility.IsSubsystemConnected())
            return false;

        ActivateXRMode();
        return true;

    }

    private static void ActivateXRMode()
    {
        IsXRModeActive = true;
        OnXRModeActivated?.Invoke();
    }
}
