using UnityEngine;


namespace Studio.Game
{
    public class UICharacterListTabView : MonoBehaviour
    {
        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private UIPlayerCard[] _cards;

        private ActorPlayerCollection _playersAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }
        }


        private void Start()        
        {
            var asset = playersAsset;

            if (asset)
            {
                var count = _cards?.Length;
                var characters = asset.GetActorNames();

                for (var i = 0; i < count; ++i)
                {
                    if (_cards[i])
                    {
                        _cards[i].Construction(
                            asset.TryGetActorPlayerScriptable(characters[i], out var player)
                            ? player
                            : asset.empty, false);
                        _cards[i].SetEnableDraggableCard(false);
                    }
                }
            }
        }
    }
}