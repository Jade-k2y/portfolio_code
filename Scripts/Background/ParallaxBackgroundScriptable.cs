using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using CustomInspector;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Background/Parallax")]
    public class ParallaxBackgroundScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<ParallaxBackgroundScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [SerializeField]
        private float _strength = 1f;
        [SerializeField]
        private float _adjust = -0.02f;
        [SerializeField, Preview]
        private Sprite[] _sprites;


        public void SetParallaxBackground(IParallaxRenderer[] parallaxes)
        {
            var count = parallaxes?.Length ?? 0;

            if (0 < count)
            {
                var mainCamera = Camera.main.transform;

                for (var i = 0; i < count; ++i)
                {
                    var reverse = count - i;

                    parallaxes[i].SetViewshed(mainCamera);
                    parallaxes[i].SetSpriteLayer(
                        (1f / reverse * _strength) + _adjust,
                        reverse,
                        i < _sprites?.Length ? _sprites[i] : null);
                }
            }
        }
    }
}