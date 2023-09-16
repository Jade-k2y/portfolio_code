using UnityEngine;
using System;


namespace Studio.Game
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField]
        private GearScriptable.AssetReference _gear;
        [SerializeField]
        private UIGearCard[] _gearCards;

        private GearScriptable _gearAsset;

        public GearScriptable gearAsset => AssetScriptable.GetLoadAsset(_gear, ref _gearAsset);


        private void OnValidate()
        {
            if (2 != _gearCards?.Length)
            {
                Array.Resize(ref _gearCards, 2);
            }
        }


        private void OnDestroy()
        {
            if (_gearAsset)
            {
                AssetScriptable.ReleaseAsset(_gear);
            }
        }


        private void Start() => OnItemToggled(0);


        public void OnItemToggled(int index)
        {
            if (gearAsset)
            {
                var even = 0 == (index % 2);
                var card = _gearCards[even ? 0 : 1];
                var other = _gearCards[even ? 1 : 0];

                if (card)
                {
                    var user = Global<User>.instance;

                    gearAsset.SetUIGearCard(card, index, user ? user.GetGearLevel(index) : 1, false);
                    card.Fading(true);
                }

                if (other)
                {
                    other.Fading(false);
                }
            }
        }
    }
}