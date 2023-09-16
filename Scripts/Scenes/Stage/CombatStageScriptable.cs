using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;


namespace Studio.Game
{
    public interface IPhaseStageContext
    {
        public void RegisterStagePhase(int phase, StageActorPresenter presenter);
    }


    public interface IPhaseScriptable
    {
        void SetView(int phaseIndex);
        void SetPlayerPositioning(int phaseIndex, ICombatActorPresenter presenter);
        void InitPhaseProgress(UIPhaseProgress target);
        void UpdatePhaseProgress(UIPhaseProgress target, int phaseIndex);
    }


    public class CombatStageScriptable<T> : StageScriptable where T : ActorScriptable.AssetReference
    {
        #region opponent
        [Serializable]
        public class Opponent
        {
            [HideInInspector]
            public string name;
            [SerializeField]
            private bool _isBoss;
            [SerializeField]
            private T _actor;
            [SerializeField]
            private Vector3 _scale = new(0.5f, 0.5f, 0.5f);

            private ActorScriptable _actorAsset;

            public ActorScriptable actorAsset => AssetScriptable.GetLoadAsset(_actor, ref _actorAsset);
            public bool isBoss => _isBoss;

            public void OnDestroy()
            {
                if (_actorAsset)
                {
                    AssetScriptable.ReleaseAsset(_actor);
                }
            }

            public StageActorPresenter CreateStageActorPresenter(int formation, int level)
            {
                if (actorAsset && actorAsset.TryGenerateStageActorPresenter(formation, level, out var presenter))
                {
                    presenter.actor.transform.localScale = _scale;

                    return presenter;
                }

                return default;
            }

            public static Opponent[] GenerateExtra(int count, StageGenerator generator)
            {
                var opponents = new Opponent[count];

                for (var i = 0; i < count; ++i)
                {
                    var scale = UnityEngine.Random.Range(0.5f, 1f);
                    
                    opponents[i] = new()
                    {
                        _actor = generator.GetRandomMonster() as T,
                        _scale = new(scale, scale, scale)
                    };
                }

                return opponents;
            }
        }
        #endregion //opponent

        #region phase
        [Serializable]
        public class Phase
        {
            [HideInInspector]
            public string name;
            [SerializeField]
            private Vector2 _center;
            [SerializeField]
            private FormationScriptable.AssetReference _formation;
            [SerializeField]
            private Opponent[] _opponents;

            private FormationScriptable _formationAsset;

            public Vector2 center => _center;
            public FormationScriptable formationAsset => AssetScriptable.GetLoadAsset(_formation, ref _formationAsset);
            public bool isExistBoss
            {
                get
                {
                    var count = _opponents?.Length;

                    for (var i = 0; i < count; ++i)
                    {
                        if (_opponents[i]?.isBoss ?? false)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            public void OnValidate(int index) => name = $"PHASE-{index + 1}";

            public void OnDestroy()
            {
                if (_formationAsset)
                {
                    AssetScriptable.ReleaseAsset(_formation);
                }

                var count = _opponents?.Length;

                for (var i = 0; i < count; ++i)
                {
                    _opponents[i]?.OnDestroy();
                }
            }

            public IEnumerator SetStagePhase(int phase, int scale, IPhaseStageContext context, UIHud hud)
            {
                var count = _opponents?.Length;
                var wfef = new WaitForEndOfFrame();

                for (var i = 0; i < count; ++i)
                {
                    var opponent = _opponents[i]?.CreateStageActorPresenter(i, scale);

                    if (opponent?.actor)
                    {
                        if (formationAsset)
                        {
                            formationAsset.SetFormation(opponent.formation, opponent.actor.transform, _center);
                        }

                        yield return wfef;

                        opponent.RegisterActionPresenter(hud?.GenerateHud(_opponents[i].isBoss));
                        context?.RegisterStagePhase(phase, opponent);
                    }
                }
            }

            public static Phase[] GenerateExtra(CombatStageScriptable<T> scriptable, StageGenerator generator)
            {
                if (scriptable && generator)
                {
                    var templates = new float[,]
                    {
                        { 0f, 0f, 0f, 0f, 0f },
                        { -7f, 7f, 0f, 0f, 0f },
                        { -7f, 0f, 7f, 0f, 0f },
                        { -14f, -7f, 0f, 7f, 0f },
                        { -14f, -7f, 0f, 7f, 14f }
                    };
                    var max = 5;
                    var count = UnityEngine.Random.Range(1, max + 1);

                    scriptable._phases = new Phase[count];

                    for (var i = 0; i < count; ++i)
                    {
                        scriptable._phases[i] = new()
                        {
                            _center = new(templates[count - 1, i], 0f),
                            _formation = generator.GetRandomFormationReference()
                        };

                        if (scriptable._phases[i].formationAsset)
                        {
                            var size = scriptable._phases[i].formationAsset.count;
                            scriptable._phases[i]._opponents = Opponent.GenerateExtra(size, generator);
                        }
                    }

                    scriptable._startCameraPosition = scriptable._phases[0]._center;

                    return scriptable._phases;
                }

                return null;
            }
        }
        #endregion //phase

        [SerializeField]
        protected BackgroundScriptable.AssetReference _background;
        [SerializeField]
        protected FormationScriptable.AssetReference _playerFormation;
        [SerializeField]
        protected Vector2 _startCameraPosition = Vector2.zero;
        [SerializeField]
        protected float _startDistance = 10f, _moveDuration = 0.5f;
        [SerializeField]
        protected Phase[] _phases;

        private BackgroundScriptable _backgroundAsset;
        private FormationScriptable _playerFormationAsset;

        public BackgroundScriptable backgroundAsset => AssetScriptable.GetLoadAsset(_background, ref _backgroundAsset);
        public FormationScriptable playerFormationAsset => AssetScriptable.GetLoadAsset(_playerFormation, ref _playerFormationAsset);


        protected override bool IsValidate(GameScene scene) => false;


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_backgroundAsset)
            {
                AssetScriptable.ReleaseAsset(_background);
            }

            if (_playerFormationAsset)
            {
                AssetScriptable.ReleaseAsset(_playerFormation);
            }

            var count = _phases?.Length;

            for (var i = 0; i < count; ++i)
            {
                _phases[i]?.OnDestroy();
            }
        }


        public override StageScriptable GetExtra(GameScene gameScene, string title, StageGenerator generator)
        {
            var extra = base.GetExtra(gameScene, title, generator) as CombatStageScriptable<T>;

            if (generator)
            {
                _background = generator.GetRandomBackgroundReference();
                _playerFormation = generator.GetPlayerFormationReference(_gameScene);
            }

            extra._phases = Phase.GenerateExtra(extra, generator);

            return extra;
        }


        public void InitPhaseProgress(UIPhaseProgress target)
        {
            target?.InitProgress(_phases.Select(x => x.isExistBoss).ToArray());
        }


        public void UpdatePhaseProgress(UIPhaseProgress target, int phaseIndex)
        {
            target?.UpdateProgress(_phases?.Length ?? 0, phaseIndex, 0.5f);
        }


        public void InitView()
        {
            var mainCamera = Camera.main;

            if (mainCamera)
            {
                mainCamera.transform.position += new Vector3(_startCameraPosition.x, _startCameraPosition.y, 0f);
            }
        }


        public void SetThumbnail(Image thumbnail)
        {
            if (backgroundAsset)
            {
                backgroundAsset.SetThumbnail(thumbnail);
            }
        }


        public void SetGround(SpriteRenderer renderer)
        {
            if (backgroundAsset)
            {
                backgroundAsset.SetGround(renderer);
            }
        }


        public IEnumerator SetActorPlayers(IActorPresenterRepository repository, IActorCollection collection, UIHud hud)
        {
            if (Global<User>.instance.TryGetTeamPlayers(out var teamPlayers))
            {
                var wfef = new WaitForEndOfFrame();
                var count = teamPlayers?.Count;

                for (var i = 0; i < count; ++i)
                {
                    if (teamPlayers[i]?.isExist ?? false)
                    {
                        var presenter = collection?.GenerateStageActorPresenter(teamPlayers[i].actorName, i, teamPlayers[i].growth.level);

                        if (presenter is not null)
                        {
                            if (playerFormationAsset && presenter.actor)
                            {
                                var center = 0 < _phases?.Length ? _phases[0].center : Vector2.zero;

                                if (0f != _startDistance)
                                {
                                    center += (Vector2.left * _startDistance);
                                }

                                playerFormationAsset.SetFormation(presenter.formation, presenter.actor.transform, center);
                            }

                            yield return wfef;

                            var gui = hud?.GenerateHud(false);

                            if (gui is IHealthBarGUI healthBar && collection.TryGetActorPlayerScriptable(presenter.actorName, out var scriptable))
                            {
                                scriptable.SetRepresentColor(healthBar.bar);
                            }

                            presenter.RegisterActionPresenter(gui);
                            repository?.RegisterActorPresenter(presenter);
                        }
                    }
                }
            }
        }


        public IEnumerator SetStagePhases(int scale, IPhaseStageContext context, UIHud hud)
        {
            var count = _phases?.Length;

            for (var i = 0; i < count; ++i)
            {
                yield return _phases[i].SetStagePhase(i, scale, context, hud);
            }
        }


        public void SetView(int phaseIndex)
        {
            var mainCamera = Camera.main;

            if (mainCamera && (0 <= phaseIndex && phaseIndex < _phases?.Length))
            {
                var pos = mainCamera.transform.position;

                mainCamera.transform.DOMove(new(_phases[phaseIndex].center.x, pos.y, pos.z), 0.5f)
                    .SetLink(mainCamera.gameObject);
            }
        }


        public void SetPlayerPositioning(int phaseIndex, ICombatActorPresenter presenter)
        {
            if (playerFormationAsset && presenter?.actor)
            {
                presenter.DoRun();

                playerFormationAsset.MoveFormation(
                    presenter.formation,
                    presenter.actor.transform,
                    0 <= phaseIndex && phaseIndex < _phases?.Length ? _phases[phaseIndex].center : Vector2.zero,
                    _moveDuration,
                    () => presenter?.DoStandby());
            }
        }
    }
}