using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _timeToHide = 1f;
    [SerializeField] private Image _loadingImage;

    public static LoadingScreen Instance { get; private set; }

    public IEnumerator FadeIn(bool force = false)
    {
        if (force)
        {
            _canvasGroup.alpha = 1f;
        }
        else
        {
            yield return StartCoroutine(LerpRoutine(0f, 1f, _timeToHide));
        }
    }

    public IEnumerator FadeOut(bool force = false)
    {
        if (force)
        {
            _canvasGroup.alpha = 0f;
        }
        else
        {
            yield return StartCoroutine(LerpRoutine(1f, 0f, _timeToHide));
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        LoadingScreenController.OnLoadProgressChange += LoadingScreenController_OnLoadProgressChange;
    }

    private void LoadingScreenController_OnLoadProgressChange(float progress)
    {
        _loadingImage.fillAmount = progress;
    }

    private IEnumerator LerpRoutine(float initialValue, float newValue, float time)
    {
        float initialTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < initialTime + time)
        {
            _canvasGroup.alpha = Mathf.Lerp(initialValue, newValue, (Time.realtimeSinceStartup - initialTime) / time);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDisable()
    {
        LoadingScreenController.OnLoadProgressChange -= LoadingScreenController_OnLoadProgressChange;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
