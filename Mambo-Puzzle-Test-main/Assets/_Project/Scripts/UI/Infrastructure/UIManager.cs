using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Infractructure
{
    public class UIManager
    {
        private readonly IReadOnlyDictionary<Type, BaseWindow> _windowPrefabByType;
        private readonly Transform _canvas;

        public UIManager(IReadOnlyDictionary<Type, BaseWindow> windowPrefabByType, Transform canvas) 
        { 
            _windowPrefabByType = windowPrefabByType;
            _canvas = canvas;
        }

        public T ShowWindow<T>() where T : BaseWindow
        {
            Type type = typeof(T);
            BaseWindow windowPrefab = _windowPrefabByType[type];
            BaseWindow windowInstance = UnityEngine.Object.Instantiate(windowPrefab, _canvas);
            return windowInstance as T;
        }

        public void HideWindow(BaseWindow window)
        {
            UnityEngine.Object.Destroy(window.gameObject);
        }
    }
}
