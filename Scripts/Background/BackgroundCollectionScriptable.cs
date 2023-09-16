using UnityEngine;
#if false
using UnityEditor;
#endif


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Background/Collection")]
    public class BackgroundCollectionScriptable : CollectionScriptable<BackgroundCollectionScriptable, BackgroundScriptable.AssetReference>
    {
#if false
        public UnityEngine.Object editorFolder;


        [ContextMenu("Generate")]
        private void OnEditorGenerateList()
        {
            if (editorFolder)
            {
                var dir = AssetDatabase.GetAssetPath(editorFolder);
                var sprites = AssetDatabase.FindAssets("t:Sprite", new[] { dir });
                var count = sprites?.Length ?? 0;

                if (count != _collection?.Length)
                {
                    Array.Resize(ref _collection, count);
                }

                for (var i = 0; i < count; ++i)
                {
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(sprites[i]));
                    var assetPath = $"{dir}/{sprite.name}.asset";
                    var instance = CreateInstance<BackgroundScriptable>();

                    instance.OnEditorSetSprite(sprite);
                    AssetDatabase.CreateAsset(instance, assetPath);
                    /*
                    _collection[i] = Element.Create(instance);
                    */
                }

                editorFolder = null;
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }
}