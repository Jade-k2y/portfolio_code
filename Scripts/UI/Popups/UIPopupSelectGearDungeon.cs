using UnityEngine;
using UnityEngine.UI;
using System;


namespace Studio.Game
{
    [Popup(nameof(UIPopupSelectGearDungeon), "Popups")]
    public class UIPopupSelectGearDungeon : UIPopup
    {
        private static readonly int GearCount = 13;

        [SerializeField]
        private GearScriptable.AssetReference _gear;
        [SerializeField]
        private Button _close;
        [SerializeField]
        private UIGearCard[] _gearCards;

        private GearScriptable _gearAsset;

        public GearScriptable gearAsset => AssetScriptable.GetLoadAsset(_gear, ref _gearAsset);


        private void OnValidate()
        {
            if (GearCount != _gearCards?.Length)
            {
                Array.Resize(ref _gearCards, GearCount);
            }
        }


        private void OnDestroy()
        {
            if (_gearAsset)
            {
                AssetScriptable.ReleaseAsset(_gear);
            }
        }


        protected override void Start()
        {
            base.Start();

            if (gearAsset)
            {
                var user = Global<User>.instance;

                for (var i = 0; i < GearCount; ++i)
                {
                    gearAsset.SetUIGearCard(_gearCards[i], i, user ? user.GetGearLevel(i) : 1, true);
                }
            }

            _close.SetClickEvent(OnClose, false);
        }
    }
}