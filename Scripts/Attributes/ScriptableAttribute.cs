using UnityEngine;
using System;


namespace Studio.Game
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ScriptableAttribute : Attribute
    {
        public HideFlags hideFlags { get; private set; }


        public ScriptableAttribute(HideFlags hideFlags = HideFlags.None)
        {
            this.hideFlags = hideFlags;
        }
    }
}