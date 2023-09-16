using System;
using System.Collections.Generic;


namespace Studio.Game
{
    public interface ICombatActorPresenter
    {
        Actor actor { get; }
        int formation { get; }
        bool isUseSpirit { get; }

        void DoStandby();
        void DoRun();
        void DoCheer();
    }

    public interface ICombatActionTarget
    {
        Actor actor { get; }
        void OnTakeAction(ICombatActionModel model);
    }

    public interface ITurnBasedCombatant
    {
        bool isAlive { get; }
        bool isPlayer { get; }
        bool isActionable { get; }

        void DoAction(IList<ICombatActionTarget> members, IList<ICombatActionTarget> opponents, Action onFinished);
    }

    public interface IUseItemReceiver
    {
        void UseSpirit(bool use);
    }


    public class StageActorPresenter : ICombatActorPresenter, ITurnBasedCombatant, ICombatActionTarget, IUseItemReceiver
    {
        private readonly int _formation;
        private readonly string _actorName;
        private readonly int _level;
        private readonly Actor _actor;
        private readonly StageActorModel _model;
        private readonly List<ICombatActionPresenter> _actionPresenters = new();

        public int formation => _formation;
        public string actorName => _actorName;
        public int level => _level;
        public Actor actor => _actor;
        public bool isAlive => _model?.isAlive ?? false;
        public bool isPlayer => _actor is ActorPlayer;
        public bool isActionable => _actor is not ActorNpc;
        public bool isUseSpirit { get; private set; }


        public StageActorPresenter(int formation, string actorName, int level, Actor actor, Stats stats)
        {
            _formation = formation;
            _actorName = actorName;
            _level = level;
            _actor = actor;
            _model = new StageActorModel(stats, _actor ? _actor.attackType : AttackType.None);
        }


        public void RegisterActionPresenter(ICombatActionPresenter presenter)
        {
            if (presenter?.Initialize() ?? false)
            {
                _actionPresenters.Add(presenter);
            }
        }


        void IUseItemReceiver.UseSpirit(bool use) => isUseSpirit = use;


        void ICombatActorPresenter.DoStandby()
        {
            if (_actor)
            {
                _actor.Standby();
            }
        }


        void ICombatActorPresenter.DoRun()
        {
            if (_actor)
            {
                _actor.Run();
            }
        }


        public void DoCheer()
        {
            if (_actor)
            {
                _actor.Cheer();
            }
        }


        public void DoResurrection(Action onResurrected)
        {
            if (_actor)
            {
                _actor.Resurrection(() =>
                {
                    var hp = _model?.Resurrection();

                    foreach (var presenter in _actionPresenters)
                    {
                        presenter?.OnResurrection(hp);
                    }

                    onResurrected?.Invoke();
                });
            }
        }


        void ITurnBasedCombatant.DoAction(IList<ICombatActionTarget> members, IList<ICombatActionTarget> others, Action onFinished)
        {
            if (_actor)
            {
                var count = others?.Count ?? 0;

                if (isPlayer && isUseSpirit)
                {
                    isUseSpirit = Global<User>.instance.UseItemSpirit();
                }

                if (9999 > _level)
                {
                    _actor.Attack(others[UnityEngine.Random.Range(0, count)], _model?.GetActionModel(this), onFinished);
                }
                else
                {
                    _actor.Attack(others, _model?.GetActionModel(this), onFinished);
                }
            }
        }


        void ICombatActionTarget.OnTakeAction(ICombatActionModel model)
        {
            if (_model?.isAlive ?? false)
            {
                if (model?.actionType is CombatActionType.Attack)
                {
                    var hp = _model.TakeDamage(model.amount);

                    if (_actor)
                    {
                        _actor.TakeAttack();

                        if (!_model.isAlive)
                        {
                            _actor.Death();
                        }

                        if (_actor.hud)
                        {
                            foreach (var presenter in _actionPresenters)
                            {
                                presenter?.OnTakeAction(model, _actor.hud.position, hp);
                            }
                        }
                    }
                }
                else if (model?.actionType is CombatActionType.Skill)
                {
                    //! @todo : skill
                }
            }
        }
    }
}