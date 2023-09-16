using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    [Popup(nameof(UIPopupClearResult), "Popups")]
    public class UIPopupClearResult : UIPopup
    {
        #region models
        public class Model : IPopupModel
        {
            public long exp { get; private set; }
            public Action onExit { get; private set; }
            public Action onNext { get; private set; }

            public Model(long exp, Action onExit, Action onNext)
            {
                this.exp = exp;
                this.onExit = onExit;
                this.onNext = onNext;
            }
        }

        public class GearModel : Model
        {
            public int index { get; private set; }
            public int level { get; private set; }

            public GearModel(int index, int level, long exp, Action onExit, Action onNext)
                : base(exp, onExit, onNext)
            {
                this.index = index;
                this.level = level;
            }
        }
        #endregion

        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private GearScriptable.AssetReference _gear;
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private UIUserProfile _userProfile;
        [SerializeField]
        private UIPlayerCard[] _cards;
        [SerializeField]
        private UIGearCard _gearCard;
        [SerializeField]
        private Button _exit, _next;

        private ActorPlayerCollection _playersAsset;
        private GearScriptable _gearAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);
        public GearScriptable gearAsset => AssetScriptable.GetLoadAsset(_gear, ref _gearAsset);


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }

            if (_gearAsset)
            {
                AssetScriptable.ReleaseAsset(_gear);
            }
        }


        protected override void Start()
        {
            base.Start();

            _exit.SetClickEvent(() =>
            {
                (_model as Model).onExit?.Invoke();
                OnClose();
            });
            _next.SetClickEvent(() =>
            {
                (_model as Model).onNext?.Invoke();
                OnClose();
            });

            if (playersAsset && GameConstant.MaxPlayerSlotCount <= _cards?.Length && Global<User>.instance.TryGetTeamPlayers(out var team))
            {
                for (var i = 0; i < GameConstant.MaxPlayerSlotCount; ++i)
                {
                    if (_cards[i])
                    {
                        if (team[i]?.isExist ?? false)
                        {
                            if (playersAsset.TryGetActorPlayerScriptable(team[i]?.actorName, out var actor))
                            {
                                _cards[i].Construction(actor, false);
                            }
                        }
                        else
                        {
                            _cards[i].SetActiveLayer(false);
                        }

                        _cards[i].SetEnableDraggableCard(false);
                        _cards[i].SetEnableDroppableCard(false);
                    }
                }
            }

            if (_gearCard)
            {
                _gearCard.Fading(false, true);
            }

            SetInteractableButtons(false);
        }


        protected override void ApplyModel()
        {
            base.ApplyModel();

            if (_title)
            {
                if (_model is GearModel gearModel)
                {
                    _title.SetText($"{gearModel.level.ToLevel()} 세계 클리어");

                    Global<User>.instance.SetGearLevel(gearModel.index, gearModel.level + 1);
                }
                else
                {
                    _title.SetText("스테이지 클리어");
                }
            }
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();

            if (_userProfile)
            {
                _userProfile.OnPresentAdditionalExp((_model as Model).exp, () =>
                {
                    if (_model is GearModel gearModel)
                    {
                        if (_gearCard)
                        {
                            if (gearAsset)
                            {
                                gearAsset.SetUIGearCard(_gearCard, gearModel.index, Global<User>.instance.GetGearLevel(gearModel.index), false);
                            }

                            _gearCard.Fading(true, onComplete: () => SetInteractableButtons(true));
                            _gearCard.SetActiveFX(true);
                        }
                        else
                        {
                            SetInteractableButtons(true);
                        }
                    }
                    else
                    {
                        SetInteractableButtons(true);
                    }
                });
            }

            var count = _cards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_cards[i] && Global<User>.instance.TryGetTeamPlayers(out var team))
                {
                    _cards[i].OnPresentAdditionalExp((_model as Model).exp, null);
                }
            }
        }


        private void SetInteractableButtons(bool interactable)
        {
            if (_exit)
            {
                _exit.interactable = interactable;
            }

            if (_next)
            {
                _next.interactable = interactable;
            }
        }
    }
}