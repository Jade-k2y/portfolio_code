using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.AddressableAssets;
using System;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Environment")]
    public class EnvironmentScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<EnvironmentScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [Serializable]
        public class Weather
        {
            [SerializeField]
            private WeatherType _weatherType;
        }

        [Serializable]
        public class Light
        {
            [SerializeField]
            private Light2D.LightType _lightType;
            [SerializeField]
            private Color _color = Color.white;
            [SerializeField]
            private float _intensity;

            public void SetLight(Light2D target)
            {
                if (target)
                {
                    target.lightType = _lightType;
                    target.color = _color;
                    target.intensity = _intensity;
                }
            }
        }


        [SerializeField]
        private Weather _weather;
        [SerializeField]
        private Light _light;


        public void SetLight(Light2D target) => _light?.SetLight(target);
    }
}