using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Studio.Game
{
    public class GearStageContext : StageContext<GearStageScriptable, UIGearStage>, IPhaseStageContext
    {
        private readonly Dictionary<int, StagePhase> _phases = new();
        private int _lastPhaseIndex;
        private int _index;
        private int _level;
        private string _title;


        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var phase in _phases.Values)
            {
                phase?.Release();
            }

            _phases.Clear();
        }


        protected override void Awake()
        {
            base.Awake();

            var bridge = StageGenerator.bridge;

            _index = bridge.index;
            _level = bridge.level;
            _title = bridge.title;
            _extra = true;
            _scriptableAsset = StageGenerator.GenerateStage<GearStageScriptable>(GameScene.GEAR_STAGE, $"'{_level.ToLevel()} {_title}' 세계");

            if (guiAsset)
            {
                guiAsset.SetGearStageScriptable(scriptableAsset);
            }
        }


        protected override IEnumerator Load()
        {
            var asset = scriptableAsset;
            var gui = guiAsset;

            if (asset && gui)
            {
                yield return asset.SetActorPlayers(this, playersAsset, gui.playerHud);
                yield return gui.LoadCombatCard(_presenters, playersAsset);
                yield return asset.SetStagePhases(_level, this, gui.opponentHud);
                yield return gui.OnFadeoutScreen();
            }
            else
            {
                SceneContext.GotoContent(GameScene.HOME);
            }
        }


        protected override IEnumerator Retry()
        {
            var resurrected = 0;

            foreach (var presenter in _presenters)
            {
                presenter.Value.DoResurrection(() => resurrected++);
            }

            yield return new WaitUntil(() => resurrected == _presenters.Values.Count);
        }


        protected override IEnumerator Play()
        {
            var phases = _phases.Values.ToArray();
            var count = phases?.Length;

            while (_lastPhaseIndex < count)
            {
                var phase = phases[_lastPhaseIndex];

                if (phase is not null)
                {
                    phase.ReadyPhase();

                    if (guiAsset)
                    {
                        guiAsset.UpdatePhaseProgress(phase.phaseIndex);
                    }

                    yield return phase.PlayPhase(result => _gameResult = result);

                    if (!_gameResult)
                    {
                        yield break;
                    }
                }

                ++_lastPhaseIndex;
            }
        }


        protected override IEnumerator Finish()
        {
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

                Popup<UIPopupClearResult>.instance.OnPopup(new UIPopupClearResult.GearModel(
                    _index,
                    _level,
                    Global<Table>.instance.GetStageRewardExp(_index),
                    () => SceneContext.GotoContent(GameScene.HOME),
                    () =>
                    {
                        Popup<UIPopupConfirm>.instance.OnPopup(
                            new UIPopupConfirm.Model(
                                $"{(_level + 1).ToLevel()} {_title}",
                                "장비 세계로 입장 하시겠습니까?",
                                () =>
                                {
                                    StageGenerator.SetStageBridge(_index, _level + 1, _title);
                                    SceneContext.GotoContent(GameScene.GEAR_STAGE);
                                },
                                () => SceneContext.GotoContent(GameScene.HOME)));
                    }));
            }
            else
            {
                var gemCount = 10 * ++_tryCount;

                Popup<UIPopupContinue>.instance.OnPopup(new UIPopupContinue.Model(gemCount, () => StartCoroutine(OnStage())));
            }
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