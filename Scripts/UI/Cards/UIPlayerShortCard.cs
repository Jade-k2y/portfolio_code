using UnityEngine.UI;
using TMPro;


namespace Studio.Game
{
    public class UIPlayerShortCard : UICard<UICard.Empty, UICard.Active>, IActorPlayerShortGUI, IReplaceableCard
    {
        bool IReplaceableCard.replaceable => false;
        Graphic IActorPlayerGUI.background => _active?.background;
        Image IActorPlayerGUI.mastery => _active?.mastery;
        Image IActorPlayerGUI.thumbnail => _active?.thumbnail;
        TMP_Text IActorPlayerGUI.actorName => _active?.actorName;
        TMP_Text IActorPlayerShortGUI.level => _active?.level;

        private bool _hasPlayerCard;


        public override void Construction(ActorPlayerScriptable scriptable, bool register)
        {
            if ((this.scriptable = scriptable).Exist())
            {
                _hasPlayerCard = Global<User>.instance.HasPlayer(this.scriptable.actorName);

                this.scriptable.SetPlayerGUI(this, _hasPlayerCard);

                if (Global<User>.instance.TryGetPlayer(this.scriptable.actorName, out var player) && player.growth is not null)
                {
                    player.growth.level.ToLevel(_active?.level);
                }
                else
                {
                    if (_active?.level)
                    {
                        _active.level.SetText("LV.?");
                    }
                }

                SetActiveEyeBlock(!_hasPlayerCard);
                SetActiveLayer(true);
            }
            else
            {
                SetActiveLayer(false);
            }
        }


        public override void SetEnableDraggableCard(bool enable)
        {
            base.SetEnableDraggableCard(_hasPlayerCard && enable);
        }
    }
}