using UnityEngine;


namespace Studio.Game
{
    public class UIPvPStage : UIScenePlay<PvPStageScriptable>
    {
        [SerializeField]
        private UITimer _timer;


        public void OnStartTimer()
        {
            if (_timer)
            {
                _timer.StartTimer();
            }
        }


        public void OnPauseTimer()
        {
            if (_timer)
            {
                _timer.PauseTimer();
            }
        }
    }
}