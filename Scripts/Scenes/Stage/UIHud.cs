using UnityEngine;
using System;
using CustomInspector;


namespace Studio.Game
{
    [Serializable]
    public class UIHud
    {
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private UIHealthBarScriptable _scriptable;
        [SerializeField]
        private bool _useBoss;
        [SerializeField, ShowIf(nameof(_useBoss))]
        private UIHealthBarScriptable _bossScriptable;


        public UIHealthBar GenerateHud(bool isBoss)
        {
            var scriptable = isBoss && _useBoss ? _bossScriptable : _scriptable;

            if (scriptable)
            {
                return scriptable.Generate(_canvas, _root);
            }

            return null;
        }
    }
}