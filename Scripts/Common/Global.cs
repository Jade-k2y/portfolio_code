using UnityEngine;
using System;


namespace Studio.Game
{
    public static class Global<T> where T : MonoBehaviour
    {
        static Global()
        {
            _attribute = Attribute.GetCustomAttribute(typeof(T), typeof(GlobalAttribute)) as GlobalAttribute;
        }

        private static readonly GlobalAttribute _attribute;
        private static readonly object _lock = new();
        private static T _instance;


        public static T instance
        {
            get
            {
                if (!_instance)
                {
                    lock (_lock)
                    {
                        /*
                        if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            Debug.LogError("MonoBehaviour");
                        }
                        else if (typeof(T).IsSubclassOf(typeof(ScriptableObject)))
                        {
                            Debug.LogError("ScriptableObject");
                        }
                        */

                        if (_attribute.generate)
                        {
                            _instance = new GameObject($"[Global] {typeof(T).Name}").AddComponent<T>();
                        }
                        else
                        {
                            _instance = UnityEngine.Object.FindObjectOfType<T>();
                        }

                        if (_attribute.permanent)
                        {
                            UnityEngine.Object.DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }

                return _instance;
            }
        }
        public static bool hasInstance => _instance;


        public static void Purge()
        {
            if (hasInstance)
            {
                UnityEngine.Object.Destroy(_instance.gameObject);
                _instance = null;
            }
        }
    }
}