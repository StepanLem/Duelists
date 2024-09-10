using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingImage : MonoBehaviour
{
    [SerializeField] private Image _image;

    private void OnEnable()
    {
        LoadingScreenController.OnLoadProgressChange += UpdateImageFillAmount;
    }

    private void OnDisable()
    {
        LoadingScreenController.OnLoadProgressChange -= UpdateImageFillAmount;
    }

    private void UpdateImageFillAmount(float progress)
    {
        _image.fillAmount = progress;
    }
}
