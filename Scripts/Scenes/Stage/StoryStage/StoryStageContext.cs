using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Studio.Game
{
    public class StoryStageContext : StageContext<StoryStageScriptable, UIStoryStage>, IPhaseStageContext
    {
        [SerializeField]
        private StoryCollectionScriptable.AssetReference _stories;

        private readonly Dictionary<int, StagePhase> _phases = new();
        private int _lastPhaseIndex;
        private StoryCollectionScriptable _storiesAsset;

        public StoryCollectionScriptable storiesAsset => AssetScriptable.GetLoadAsset(_stories, ref _storiesAsset);


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_storiesAsset)
            {
                AssetScriptable.ReleaseAsset(_stories);
            }

            foreach (var phase in _phases.Values)
            {
                phase?.Release();
            }

            _phases.Clear();
        }


        protected override void Awake()
        {
            base.Awake();

            var storyIndex = Global<User>.instance.storyIndex;

            if (storiesAsset && storiesAsset.TryGetStoryContent(storyIndex, out var content))
            {
                _scriptableAsset = content as StoryStageScriptable;
            }
            else
            {
                _extra = true;
                _scriptableAsset = StageGenerator.GenerateStage<StoryStageScriptable>(GameScene.STORY_STAGE, $"CHAPTER 1-{storyIndex}");
            }
        }


        protected override void Start()
        {
            base.Start();

            if (guiAsset)
            {
                guiAsset.SetStoryStageScriptable(scriptableAsset);
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
                yield return asset.SetStagePhases(Global<User>.instance.storyIndex + 1, this, gui.opponentHud);
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

                var clearIndex = Global<User>.instance.StoryCleared();

                Popup<UIPopupClearResult>.instance.OnPopup(new UIPopupClearResult.Model(
                    Global<Table>.instance.GetStageRewardExp(clearIndex),
                    () => SceneContext.GotoContent(GameScene.HOME),
                    SceneContext.GotoStory));
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