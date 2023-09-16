using UnityEngine;


namespace Studio.Game
{
    public interface IStageActorModel
    {
        bool isAlive { get; }
    }


    public class StageActorModel : IStageActorModel
    {
        private readonly Stats _stats;
        private readonly AttackType _attackType;

        public bool isAlive => 0 < _stats?.runtimeHP?.current;
        public bool isCritical => Random.Range(0f, 100f) <= _stats?.criticalRate;
        public AttackType attackType => _attackType;

        public StageActorModel(Stats stats, AttackType attackType)
        {
            _stats = stats;
            _attackType = attackType;
        }


        public Stats.HP Resurrection() => _stats?.Resurrection(0.7f);


        public int GetDamage(bool critical, float variance = 1f)
        {
            if (_stats is not null)
            {
                var attack = (int)(_stats.attack * variance);

                return critical ? _stats.GetCriticalDamage(attack) : attack;
            }

            return 0;
        }


        public ICombatActionModel GetActionModel(ICombatActorPresenter presenter)
        {
            return _attackType switch
            {
                AttackType.Melee1H => new CombatActionModel.AttackMelee1H(this, presenter),
                AttackType.Melee2H => new CombatActionModel.AttackMelee2H(this, presenter),
                AttackType.MeleePaired => new CombatActionModel.AttackMeleePaired(this, presenter),
                AttackType.Bow => new CombatActionModel.AttackBow(this, presenter),
                AttackType.Gun => new CombatActionModel.AttackGun(this, presenter),
                AttackType.Magic => new CombatActionModel.AttackMagic(this, presenter),
                _ => new CombatActionModel.None(),
            };
        }


        public Stats.HP TakeDamage(int damage) => _stats?.TakeDamage(damage);
    }
}