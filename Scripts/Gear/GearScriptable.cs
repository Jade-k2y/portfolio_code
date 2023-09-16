using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using CustomInspector;


namespace Studio.Game
{
    public class GearScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<GearScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [Serializable]
        public class Gear
        {
            public string name;
            [SerializeField, Preview]
            private Sprite _icon;
            [SerializeField]
            private GearMastery _mastery;
            [SerializeField]
            private Stats _stats;
            [SerializeField]
            private string _ownerActorName;

            public Sprite icon => _icon;
            public GearMastery gearMastery => _mastery;
            public Stats stats => _stats;
            public string ownerActorName => _ownerActorName;
        }

        [SerializeField]
        private Gear[] _gears;


        public void SetUIGearCard(UIGearCard card, int index, int level, bool gotoDungeon)
        {
            if (card && 0 <= index && index < _gears?.Length)
            {
                card.Construction(_gears[index], index, level, gotoDungeon);
            }
        }
    }
}