using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.__Internal_Assets.UI.Architecture.GameRoot
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _sceneUIContainer;
        [SerializeField] private GameObject _loadingScreen;

        private void Awake()
        {
            HideLoadingScreen();
        }

        public void ShowLoadingScreen()
        {
            _loadingScreen.SetActive(true);
        }

        public void HideLoadingScreen()
        {
            _loadingScreen.SetActive(false);
        }

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();
            sceneUI.transform.SetParent(_sceneUIContainer, false);
        }

        public void ClearSceneUI()
        {
            int sceneUIChildCount = _sceneUIContainer.childCount;
            for (int i = 0; i < sceneUIChildCount; i++) 
            {
                Destroy(_sceneUIContainer.GetChild(i).gameObject);
            }
        }
    }
}
