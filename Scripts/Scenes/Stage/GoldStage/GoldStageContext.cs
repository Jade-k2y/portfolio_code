using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Studio.Game
{
    public class GoldStageContext : StageContext<GoldStageScriptable, UIGoldStage>, IPhaseStageContext
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
                yield return asset.SetStagePhases(10, this, gui.opponentHud);
                yield return gui.OnFadeoutScreen();
            }
            else
            {
                SceneContext.GotoContent(GameScene.HOME);
            }
        }


        protected override IEnumerator Retry()
        {
            yield break;
        }


        protected override IEnumerator Play()
        {
            var etor = _phases.Values.GetEnumerator();

            while (etor.MoveNext())
            {
                if (etor.Current is not null)
                {
                    etor.Current.ReadyPhase();

                    if (guiAsset)
                    {
                        guiAsset.UpdatePhaseProgress(etor.Current.phaseIndex);
                    }

                    yield return etor.Current.PlayPhase(result => _gameResult = result);
                }
            }

            foreach (var presenter in _presenters)
            {
                if (presenter.Value.isAlive)
                {
                    presenter.Value.DoCheer();
                }
            }
        }


        protected override IEnumerator Finish()
        {
            var wfs = new WaitForSeconds(_finishDelay);

            yield return wfs;

            GotoHome();
        }


        void IPhaseStageContext.RegisterStagePhase(int phase, StageActorPresenter presenter)
        {
            if (!_phases.ContainsKey(phase))
            {
                _phases.Add(phase, new StagePhase(phase, scriptableAsset, _presenters.Values));
            }

            _phases[phase].AddOpponent(presenter);
        }
    }
}