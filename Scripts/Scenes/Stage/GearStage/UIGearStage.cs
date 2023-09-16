
namespace Studio.Game
{
    public class UIGearStage : UIScenePlay<GearStageScriptable>
    {
        public void SetGearStageScriptable(GearStageScriptable scriptable)
        {
            if (_scriptable = scriptable)
            {
                if (_screen)
                {
                    _scriptable.SetTitle(_screen.title);
                }

                _scriptable.SetTitle(_stageInfo?.title);
                _scriptable.InitPhaseProgress(_stageInfo);
            }
        }
    }
}