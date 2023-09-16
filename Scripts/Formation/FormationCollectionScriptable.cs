using UnityEngine;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Formation/Collection")]
    public class FormationCollectionScriptable : CollectionScriptable<FormationCollectionScriptable, FormationScriptable.AssetReference>
    {
        [SerializeField]
        private FormationScriptable.AssetReference _player_3, _player_4;


        public FormationScriptable.AssetReference GetPlayerFormationReference(GameScene scene)
            => scene is GameScene.PVP_STAGE ? _player_3 : _player_4;
    }
}