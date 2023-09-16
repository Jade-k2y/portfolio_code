using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Studio.Game
{
    public class SceneStarter : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject _gui;

        private GameObject _guiAsset;


        private void OnDestroy()
        {
            if (_guiAsset)
            {
                AssetScriptable.ReleaseAsset(_gui);
            }
        }


        private void Start()
        {
            _guiAsset = Instantiate(AssetScriptable.LoadAsset<GameObject>(_gui));
        }
    }
}