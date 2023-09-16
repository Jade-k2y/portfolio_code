using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Studio.Game
{
    public class StagePhase
    {
        private readonly float _turnDelay = 1f;
        private readonly int _index;
        private readonly IPhaseScriptable _scriptable;
        private readonly List<StageActorPresenter> _players = new();
        private readonly List<StageActorPresenter> _opponents = new();
        private readonly List<ITurnBasedCombatant> _combatants = new();

        public int phaseIndex => _index;


        public StagePhase(int phaseIndex, IPhaseScriptable scriptable, IEnumerable<StageActorPresenter> players)
        {
            _index = phaseIndex;
            _scriptable = scriptable;
            _players.AddRange(players);
            _combatants.AddRange(_players);
        }


        public void Release() => _opponents?.Clear();


        public void ReadyPhase()
        {
            if (_scriptable is not null)
            {
                _scriptable.SetView(_index);

                foreach (var player in _players)
                {
                    if (player.isAlive)
                    {
                        _scriptable.SetPlayerPositioning(phaseIndex, player);
                    }
                }

                foreach (var opponent in _opponents)
                {
                    if (opponent?.actor)
                    {
                        opponent.actor.SetVisible(true);
                    }
                }
            }
        }


        public IEnumerator PlayPhase(Action<bool> onResult)
        {
            var wfs = new WaitForSeconds(_turnDelay);
            var index = 0;

            while (true)
            {
                var combatant = _combatants[index];

                if (combatant?.isActionable ?? false)
                {
                    yield return wfs;

                    var finished = false;
                    var players = _players.Where(x => x.isAlive)
                        .Select(x => x as ICombatActionTarget)
                        .ToList();
                    var opponents = _opponents.Where(x => x.isAlive)
                        .Select(x => x as ICombatActionTarget)
                        .ToList();

                    if (0 >= players.Count)
                    {
                        onResult?.Invoke(false);
                        yield break;
                    }
                    else if (0 >= opponents.Count)
                    {
                        onResult?.Invoke(true);
                        yield break;
                    }

                    if (_players.Contains(combatant))
                    {
                        combatant.DoAction(players, opponents, () => finished = true);
                    }
                    else
                    {
                        combatant.DoAction(opponents, players, () => finished = true);
                    }

                    yield return new WaitUntil(() => finished);
                }

                index++;

                if (index >= _combatants.Count)
                {
                    index = 0;
                }

                while (!_combatants[index].isAlive)
                {
                    index++;

                    if (index >= _combatants.Count)
                    {
                        index = 0;
                    }
                }
            }
        }


        public void AddOpponent(StageActorPresenter opponent)
        {
            if (opponent is not null)
            {
                _opponents.Add(opponent);
                _combatants.Add(opponent);

                if (opponent.actor && 0 < _index)
                {
                    opponent.actor.SetVisible(false);
                }
            }
        }
    }
}