using UnityEngine;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Stage/Gear Dungeon")]
    public class GearStageScriptable : CombatStageScriptable<ActorScriptable.AssetReference>, IPhaseScriptable
    {
        protected override bool IsValidate(GameScene scene) => scene is GameScene.GEAR_STAGE;
    }
}