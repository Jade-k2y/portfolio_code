#if false
using UnityEngine;
using System;


namespace Studio.Game
{
    public static class Scriptable<T> where T : ScriptableObject
    {
        static Scriptable()
        {
            _attribute = Attribute.GetCustomAttribute(typeof(T), typeof(ScriptableAttribute)) as ScriptableAttribute;
        }
           private static readonly ScriptableAttribute _attribute;
        private static T _first;


        public static T first
        {
            get
            {
                if (!_first)
                {
                    var results = Resources.LoadAll<T>(string.Empty);

                    if (0 < results?.Length)
                    {
                        _first = results[0];
                        //_first.hideFlags = _attribute.hideFlags;
                    }
                }

                return _first;
            }
        }
    }
}
#endif