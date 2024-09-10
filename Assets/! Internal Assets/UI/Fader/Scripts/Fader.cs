using System;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public bool IsFading { get; private set; }

    private Action _fadedInCallback;
    private Action _fadedOutCallback;

    private const string isFaded = "faded";

    public void FadeIn(Action fadedInCallback)
    {
        if (IsFading)
        {
            return;
        }

        IsFading = true;
        _fadedInCallback = fadedInCallback;
        _animator.SetBool(isFaded, true);
    }

    public void FadeOut(Action fadedOutCallback)
    {
        if (IsFading)
        {
            return;
        }

        IsFading = true;
        _fadedOutCallback = fadedOutCallback;
        _animator.SetBool(isFaded, false);
    }

    private void Handle_FadeInAnimationOver()
    {
        _fadedInCallback?.Invoke();
        _fadedInCallback = null;
        IsFading = false;
    }

    private void Handle_FadeOutAnimationOver()
    {
        _fadedOutCallback?.Invoke();
        _fadedOutCallback = null;
        IsFading = false;
    }
}
