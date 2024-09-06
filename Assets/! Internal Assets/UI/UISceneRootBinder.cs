﻿using R3;
using System;
using Trisibo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneRootBinder : MonoBehaviour
{
    public void HandleStartButtonClick(SceneFieldReference sceneRef)
    {
        SceneManager.LoadScene(sceneRef.SceneField.BuildIndex);
    }
}
