using UnityEngine;
using UnityEngine.Rendering.Universal;
using CustomInspector;


namespace Studio.Game
{
    [Global(false)]
    public class Environment : MonoBehaviour
    {
        [SerializeField]
        private Light2D _mainLight;
        [SerializeField, Indent(1)]
        private EnvironmentScriptable _default;

        public Light2D mainLight => _mainLight;


        private void Start() => SetDefault();


        public void SetDefault()
        {
            if (_default)
            {
                _default.SetLight(_mainLight);
            }
        }
    }
}