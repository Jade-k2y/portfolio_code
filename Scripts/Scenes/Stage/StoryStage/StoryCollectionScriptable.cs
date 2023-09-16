using UnityEngine;
using UnityEngine.AddressableAssets;
using System;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Story/Main")]
    public class StoryCollectionScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<StoryCollectionScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [Serializable]
        public class Content
        {
            [HideInInspector]
            public string name;
            [SerializeField]
            private StageScriptable.AssetReference _stage;

            private StageScriptable _stageAsset;

            public StageScriptable stageAsset => AssetScriptable.GetLoadAsset(_stage, ref _stageAsset);

            public void OnDestroy()
            {
                if (_stageAsset)
                {
                    AssetScriptable.ReleaseAsset(_stage);
                }
            }
        }

        [SerializeField]
        private Content[] _contents;


        private void OnDestroy()
        {
            var count = _contents?.Length;

            for (var i = 0; i < count; ++i)
            {
                _contents[i]?.OnDestroy();
            }
        }


        public bool TryGetStoryContent(int index, out StageScriptable content)
            => content = 0 <= index && index < _contents?.Length ? _contents[index].stageAsset : null;
    }
}