using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using CustomInspector;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Background/Stage")]
    public class BackgroundScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<BackgroundScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }


        [SerializeField, Preview]
        private Sprite _sprite;
        [SerializeField]
        private Vector3 _position = new(0f, 5f, 0f);
        [SerializeField]
        private Vector3 _scale = new(2.5f, 2.5f, 2.5f);


        public void OnEditorSetSprite(Sprite sprite)
        {
#if UNITY_EDITOR
            _sprite = sprite;
#endif
        }


        public void SetThumbnail(Image thumbnail)
        {
            if (thumbnail)
            {
                thumbnail.sprite = _sprite;
            }
        }


        public void SetGround(SpriteRenderer renderer)
        {
            if (renderer)
            {
                renderer.transform.position = _position;
                renderer.transform.localScale = _scale;
                renderer.enabled = renderer.sprite = _sprite;
            }
        }
    }
}