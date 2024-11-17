using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResumeGameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _delayText;
    [Space]
    [SerializeField] private int _delay = 5;
    [SerializeField] private UnityEvent _afterDelay;

    private void OnEnable()
    {
        _delayText.text = _delay.ToString();
        StartCoroutine(DelayRoutine());
    }

    private IEnumerator DelayRoutine()
    {
        for (int i = 0; i < _delay; i++) 
        { 
            yield return new WaitForSecondsRealtime(1);
            _delayText.text = (_delay - i - 1).ToString();
        }
        _afterDelay?.Invoke();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
