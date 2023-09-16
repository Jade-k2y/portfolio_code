using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using CustomInspector;


namespace Studio.Game
{
    public class UIPlayerCard : UICard<UICard.Empty, UIPlayerCard.Active>, IActorPlayerGUI, IStatsGUI, IReplaceableCard
    {
        [Serializable]
        public class Active : UICard.Active
        {
            public Image exp;
            [Indent(1)]
            public TMP_Text hp, atk, def, @int, res, spd, crt, crd;
        }

        bool IReplaceableCard.replaceable => true;
        Graphic IActorPlayerGUI.background => _active?.background;
        Image IActorPlayerGUI.mastery => _active?.mastery;
        Image IActorPlayerGUI.thumbnail => _active?.thumbnail;
        TMP_Text IActorPlayerGUI.actorName => _active?.actorName;
        TMP_Text IStatsGUI.hp => _active?.hp;
        TMP_Text IStatsGUI.atk => _active?.atk;
        TMP_Text IStatsGUI.def => _active?.def;
        TMP_Text IStatsGUI.@int => _active?.@int;
        TMP_Text IStatsGUI.res => _active?.res;
        TMP_Text IStatsGUI.spd => _active?.spd;
        TMP_Text IStatsGUI.crt => _active?.crt;
        TMP_Text IStatsGUI.crd => _active?.crd;

        private int _index;


        protected override void Start()
        {
            base.Start();

            _index = transform.parent.GetSiblingIndex();
        }


        public override void Construction(ActorPlayerScriptable scriptable, bool register)
        {
            if ((this.scriptable = scriptable).Exist())
            {
                this.scriptable.SetPlayerGUI(this, Global<User>.instance.HasPlayer(this.scriptable.actorName));

                UpdateStats();
                SetActiveLayer(true);
                SetEnableDraggableCard(true);

                if (Global<User>.instance.TryGetPlayer(this.scriptable.actorName, out var player) && player.growth is not null)
                {
                    player.growth.level.ToLevel(_active?.level);

                    var required = Global<Table>.instance.GetPlayerLevelRequiredExp(player.growth.level);
                    var percentage = 0 < player.growth.exp ? (float)player.growth.exp / required : 0f;

                    if (_active?.exp)
                    {
                        _active.exp.fillAmount = percentage;
                    }

                    if (register)
                    {
                        Global<User>.instance.SetTeamPlayer(_index, player);
                    }

                    SetActiveEyeBlock(false);
                }
                else
                {
                    if (_active?.level)
                    {
                        _active.level.SetText("LV.?");
                    }

                    if (_active?.exp)
                    {
                        _active.exp.fillAmount = 0f;
                    }

                    SetActiveEyeBlock(true);
                }
            }
            else
            {
                SetActiveLayer(false);
                SetEnableDraggableCard(false);

                if (register)
                {
                    Global<User>.instance.SetTeamPlayer(_index, TeamPlayer.Empty());
                }
            }
        }


        public void OnPresentAdditionalExp(long additionalExp, Action onFinished)
        {
            StartCoroutine(PresentAdditionalExp(new WaitForSeconds(1f), additionalExp, onFinished));
        }


        private IEnumerator PresentAdditionalExp(YieldInstruction instruction, long additionalExp, Action onFinished)
        {
            yield return instruction;

            if (scriptable && Global<User>.instance.TryGetPlayer(scriptable.actorName, out var player))
            {
                var fromStats = new Stats(scriptable.stats, player.growth.level);

                yield return Global<Table>.instance.PresentPlayerAdditionalExp(_active?.level, _active?.exp, player.growth, additionalExp, () =>
                {
                    UpdateStats(fromStats);
                    onFinished?.Invoke();
                });
            }
            else
            {
                onFinished?.Invoke();
            }
        }

        private void UpdateStats(Stats from = null)
        {
            if (scriptable)
            {
                scriptable.SetStatsGUI(
                    this, 
                    Global<User>.instance.GetPlayerGrowth(scriptable.actorName).level, 
                    from,
                    Global<User>.instance.HasPlayer(scriptable.actorName));
            }
        }
    }
}