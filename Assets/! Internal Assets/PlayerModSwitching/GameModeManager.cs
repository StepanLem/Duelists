public class GameModeManager
{
    public bool IsXRModeActive { get; private set; }

    public bool TryActivateXRMode()
    {
        if (XRDisplayDetectionUtility.IsSubsystemConnected())
        {
            ActivateXRMode();
            return true;
        }
        else return false;
    }

    private void ActivateXRMode()
    {
        IsXRModeActive = true;
        //event?.invoke();
    }
}