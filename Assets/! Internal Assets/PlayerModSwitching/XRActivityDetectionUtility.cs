using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;

public static class XRActivityDetectionUtility
{
    /// <summary>
    /// Определяет, подключена ли XR subsystem(В случае ПК: подключён ли XR-шлем).
    /// </summary>
    /// <returns>Возвращает true, если подключена XR subsystem.</returns>
    public static bool IsSubsystemConnected()
    {
        List<XRDisplaySubsystem> xrDisplaySystems = new();
        SubsystemManager.GetSubsystems(xrDisplaySystems);

        if (xrDisplaySystems.Count > 0)
            return true;

        return false;
    }

    /// <summary>
    /// Определяет, запущена ли XR subsystem(В случае ПК: выходил ли XR-шлем из спящего режима с момента активации подсистемы).
    /// </summary>
    /// <returns>Возвращает true, если подключена и активна XR subsystem</returns>
    public static bool IsSubsystemRunning()
    {
        List<XRDisplaySubsystem> xrDisplaySystems = new();
        SubsystemManager.GetSubsystems(xrDisplaySystems);

        foreach (var system in xrDisplaySystems)
        {
            if (system.running) return true;
        }

        return false;
    }
}