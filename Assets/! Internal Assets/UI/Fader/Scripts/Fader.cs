using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private static Fader _instance;
    public static Fader Instance
    {
        get
        {
            if (!_instance)
            {
                GameObject prefab = Resources.Load<GameObject>(FaderPath);
                _instance = Instantiate(prefab).GetComponentInChildren<Fader>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    public bool IsFading { get; private set; }

    private const string FaderPath = "Fader";
    private const string isFaded = "faded";

    public void FadeIn()
    {
        if (IsFading)
        {
            return;
        }

        IsFading = true;
        _animator.SetBool(isFaded, true);
    }

    public void FadeOut()
    {
        if (IsFading)
        {
            return;
        }

        IsFading = true;
        _animator.SetBool(isFaded, false);
    }

    private void Handle_FadeInAnimationOver()
    {
        IsFading = false;
    }

    private void Handle_FadeOutAnimationOver()
    {
        IsFading = false;
    }
}
