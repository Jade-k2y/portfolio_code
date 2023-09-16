using UnityEngine;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Stage/PVP")]
    public class PvPStageScriptable : CombatStageScriptable<ActorScriptable.AssetReference>, IPhaseScriptable
    {
        protected override bool IsValidate(GameScene scene) => scene is GameScene.PVP_STAGE;
    }
}