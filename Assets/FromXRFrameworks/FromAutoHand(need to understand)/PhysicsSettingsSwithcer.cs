using UnityEngine;

//TODO: Надо понять что на практике меняют эти настройки. То есть отличие максимальной от дефолтных.
//попробовать все настройки и разобраться что каждая делает на практике.
public class PhysicsSettingsSwithcer : MonoBehaviour
{
    [SerializeField] private int _quality;

    [ContextMenu("SetPhysicsQuality")]
    private void SetPhysicsQuality()
    {
        SetPhysicsSettings(_quality);
    }

    public static void SetPhysicsSettings(int quality)
    {
        switch (quality)
        {
            case 0://Low (not recommended)  
                Time.fixedDeltaTime = 1 / 50f;
                Physics.defaultContactOffset = 0.01f;
                Physics.defaultSolverIterations = 10;
                Physics.defaultSolverVelocityIterations = 5;
                Physics.defaultMaxAngularSpeed = 35f;
                break;

            case 1://Medium (usually not recommended)
                //EnableAdaptiveForce();
                Time.fixedDeltaTime = 1 / 60f;
                Physics.defaultContactOffset = 0.0075f;
                Physics.defaultSolverIterations = 10;
                Physics.defaultSolverVelocityIterations = 5;
                Physics.defaultMaxAngularSpeed = 35f;
                break;

            case 2://High (Quest 2 or PC)
                //EnableAdaptiveForce();
                Time.fixedDeltaTime = 1 / 72f;
                Physics.defaultContactOffset = 0.005f;
                Physics.defaultSolverIterations = 20;
                Physics.defaultSolverVelocityIterations = 10;
                Physics.defaultMaxAngularSpeed = 35f;
                break;

            case 3://Very High (PC)
                //EnableAdaptiveForce(); - хз что это
                Time.fixedDeltaTime = 1 / 90f;
                Physics.defaultContactOffset = 0.0035f;
                Physics.defaultSolverIterations = 30;
                Physics.defaultSolverVelocityIterations = 20;
                Physics.defaultMaxAngularSpeed = 35f;
                break;

            default:
                Debug.Log("Поднастроек для этого уровня качества нет");
                break;
        }
    }
}
