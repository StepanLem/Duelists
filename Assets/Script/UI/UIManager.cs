using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.UI
{
    internal class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] Elements;
        [SerializeField] private Canvas Canvas;
        private Dictionary<string, GameObject> _elementsByName;
        private Dictionary<string, GameObject> _instances = new ();

        public void Start()
        {
            _elementsByName = Elements.ToDictionary(it => it.name, it => it);
        }

        public void Show(string name)
        {
            if (_elementsByName.TryGetValue(name, out var prefab))
            {
                var obj = Instantiate(prefab, Canvas.transform);
                _instances.Add(name, obj);
            }
        }
        public void Hide(string name)
        {
            if (_instances.TryGetValue(name, out var obj))
            {
                _instances.Remove(name);
                Destroy(obj);
            }
        }
    }
}
