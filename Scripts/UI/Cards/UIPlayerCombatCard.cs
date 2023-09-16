using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using CustomInspector;


namespace Studio.Game
{
    public class UIPlayerCombatCard : UICard<UICard.Empty, UIPlayerCombatCard.Active>, IActorPlayerCombatGUI, ICombatActionPresenter
    {
        [Serializable]
        public class Active : UICard.Active
        {
            [Indent(1)]
            public StatsGUI hp, sp;
        }
        
        [SerializeField]
        private float _fillDuration;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Sprite _iconPlayer, _iconFriend;
        [SerializeField]
        private bool _isFriendSlot;
        [SerializeField]
        private UIItemPotion _itemPotion;
        [SerializeField]
        private UIItemSpirit _itemSpirit;

        IUseItemReceiver _receiver;

        Graphic IActorPlayerGUI.background => _active?.background;
        Image IActorPlayerGUI.mastery => _active?.mastery;
        Image IActorPlayerGUI.thumbnail => _active?.thumbnail;
        TMP_Text IActorPlayerGUI.actorName => _active?.actorName;
        StatsGUI IActorPlayerCombatGUI.hp => _active?.hp;
        StatsGUI IActorPlayerCombatGUI.sp => _active?.sp;


        protected override void Start()
        {
            base.Start();

            if (_icon)
            {
                _icon.enabled = _icon.sprite = _isFriendSlot ? _iconFriend : _iconPlayer;
                _icon.SetNativeSize();
            }
        }


        public override void Construction(ActorPlayerScriptable scriptable, bool register)
        {
            var exist = (this.scriptable = scriptable).Exist();

            if (exist)
            {
                this.scriptable.SetPlayerCombatGUI(this, Global<User>.instance.GetPlayerGrowth(this.scriptable.actorName).level);   
            }

            SetActiveLayer(exist);

            if (_itemPotion)
            {
                _itemPotion.SetActive(exist);
            }

            if (_itemSpirit)
            {
                _itemSpirit.SetActive(exist);
            }
        }


        public void SetUseItemReceiver(IUseItemReceiver receiver) => _receiver = receiver;


        public void OnUseItemSpirit(bool use) => _receiver?.UseSpirit(use);


        bool ICombatActionPresenter.Initialize()
        {
            var result = _active?.hp?.bar;

            if (result)
            {
                _active.hp.bar.fillAmount = 1f;
            }
            
            SetActiveFocus(false);
            
            return true;
        }


        void ICombatActionPresenter.OnResurrection(Stats.HP hp)
        {
            if (hp is not null)
            {
                if (_active?.hp?.value)
                {
                    _active.hp.value.DOCounter(0, hp.current, _fillDuration)
                        .SetLink(gameObject);
                }

                if (_active?.hp?.bar)
                {
                    _active.hp.bar.DOFillAmount(hp.rate, _fillDuration)
                        .SetLink(gameObject);
                }
            }
        }


        void ICombatActionPresenter.OnTakeAction(ICombatActionModel model, Vector3 anchor, Stats.HP hp)
        {
            if (hp is not null)
            {
                if (model?.actionType is CombatActionType.Attack)
                {
                    if (_active?.hp?.value)
                    {
                        _active.hp.value.DOCounter(hp.current - hp.changed, hp.current, _fillDuration)
                            .SetLink(gameObject);
                    }

                    if (_active?.hp?.bar)
                    {
                        _active.hp.bar.DOFillAmount(hp.rate, _fillDuration)
                            .SetLink(gameObject);
                    }
                }
            }
        }
    }
}