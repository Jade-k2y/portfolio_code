using UnityEngine;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Stage/Gold Dungeon")]
    public class GoldStageScriptable : CombatStageScriptable<ActorScriptable.AssetReference>, IPhaseScriptable
    {
        protected override bool IsValidate(GameScene scene) => scene is GameScene.GOLD_STAGE;
    }
}