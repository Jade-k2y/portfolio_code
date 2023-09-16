using UnityEngine;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Stage/Story")]
    public class StoryStageScriptable : CombatStageScriptable<ActorScriptable.AssetReference>, IPhaseScriptable
    {
        protected override bool IsValidate(GameScene scene) => scene is GameScene.STORY_STAGE;


        private void OnValidate()
        {
            var count = _phases?.Length;

            for (var i = 0; i < count; ++i)
            {
                _phases[i].OnValidate(i);
            }
        }
    }
}