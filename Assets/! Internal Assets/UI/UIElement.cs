using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.UI
{
    internal class UIElement : MonoBehaviour
    {
        public UnityEvent OnShow;
        public UnityEvent OnHide;
        [SerializeField] private bool _showOnStart = false; 

        public void Show()
        {
            gameObject.SetActive(true);
            OnShow.Invoke();
        }

        public void Hide()
        {
            OnHide.Invoke();
            gameObject.SetActive(false);
        }

        public void Start()
        {
            if (!_showOnStart)
            {
                Hide();
            }
        }
    }
}
