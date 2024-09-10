using System.Collections;
using UnityEngine;

public sealed class AsyncProcessor : MonoBehaviour
{
    private static AsyncProcessor _instance;

    public static AsyncProcessor Instance
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("[AsyncProcessor]");
                _instance = gameObject.AddComponent<AsyncProcessor>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    public static Coroutine StartRoutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void StopRoutine(IEnumerator routine)
    {
        Instance.StopCoroutine(routine);
    }
}