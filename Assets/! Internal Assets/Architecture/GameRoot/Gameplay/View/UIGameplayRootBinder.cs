using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.__Internal_Assets.Architecture.Gameplay.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        private Subject<Unit> _exitSceneSignalSubj;

        public void HandleGoToMainMenuButtonCkick()
        {
            _exitSceneSignalSubj?.OnNext(Unit.Default);
        }

        public void Bind(Subject<Unit> exitSceneSignalSubj)
        {
            _exitSceneSignalSubj = exitSceneSignalSubj;
        }
    }
}
