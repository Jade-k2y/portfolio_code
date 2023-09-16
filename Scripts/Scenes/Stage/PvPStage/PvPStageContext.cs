using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Studio.Game
{
    public class PvPStageContext : StageContext<PvPStageScriptable, UIPvPStage>, IPhaseStageContext
    {
        private readonly Dictionary<int, StagePhase> _phases = new();


        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var phase in _phases.Values)
            {
                phase?.Release();
            }

            _phases.Clear();
        }


        protected override IEnumerator Load()
        {
            var asset = scriptableAsset;
            var gui = guiAsset;

            if (asset && gui)
            {
                yield return asset.SetActorPlayers(this, playersAsset, gui.playerHud);
                yield return gui.LoadCombatCard(_presenters, playersAsset);
                yield return asset.SetStagePhases(99999, this, gui.opponentHud);
                yield return gui.OnFadeoutScreen();
            }
            else
            {
                SceneContext.GotoContent(GameScene.HOME);
            }
        }


        protected override IEnumerator Retry() { yield break; }


        protected override IEnumerator Play()
        {
            var phases = _phases.Values.ToArray();
            var count = phases?.Length;
            var phaseIndex = 0;

            while (phaseIndex < count)
            {
                var phase = phases[phaseIndex];

                if (phase is not null)
                {
                    phase.ReadyPhase();

                    if (guiAsset)
                    {
                        guiAsset.OnStartTimer();
                        guiAsset.UpdatePhaseProgress(phase.phaseIndex);
                    }

                    yield return phase.PlayPhase(result => _gameResult = result);

                    if (!_gameResult)
                    {
                        yield break;
                    }
                }

                ++phaseIndex;
            }
        }


        protected override IEnumerator Finish()
        {
            if (guiAsset)
            {
                guiAsset.OnPauseTimer();
            }

            if (_gameResult)
            {
                foreach (var presenter in _presenters)
                {
                    if (presenter.Value.isAlive)
                    {
                        presenter.Value.DoCheer();
                    }
                }

                var wfs = new WaitForSeconds(_finishDelay);

                yield return wfs;

                SceneContext.GotoContent(GameScene.HOME);
                /*
                var clearIndex = Global<User>.instance.StoryCleared();

                Popup<UIPopupClearResult>.instance.OnPopup(new UIPopupClearResult.Model(
                    Global<Table>.instance.GetStageRewardExp(clearIndex + 1),
                    () => SceneContext.GotoContent(GameScene.HOME),
                    SceneContext.GotoStory));
                */
            }
            else
            {
                Popup<UIPopupPvPDefeat>.instance.OnPopup();
            }
        }


        void IPhaseStageContext.RegisterStagePhase(int phase, StageActorPresenter presenter)
        {
            if (!_phases.ContainsKey(phase))
            {
                _phases.Add(phase, new StagePhase(phase, scriptableAsset, _presenters.Values));
            }

            if (presenter.actor is ActorPlayer)
            {
                presenter.actor.Flip();
            }

            _phases[phase].AddOpponent(presenter);
        }
    }
}