using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using TMPro;
using CustomInspector;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/UI/Mastery")]
    public class MasteryScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<MasteryScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [Serializable]
        public class MasteryIcon
        {
            [HideInInspector]
            public string name;
            [SerializeField]
            private GearMastery _mastery;
            [SerializeField, Preview]
            private Sprite _icon;

            public GearMastery mastery => _mastery;

            public void OnValidate() => name = _mastery.ToString();

            public void SetIcon(Image target)
            {
                if (target)
                {
                    target.sprite = _icon;
                    target.enabled = target.sprite;
                }
            }
        }


        [SerializeField]
        private MasteryIcon[] _masteryIcons;


        public void SetMasteryIcon(Image target, GearMastery mastery)
        {
            var count = _masteryIcons?.Length;

            if (target && _masteryIcons is not null)
            {
                for (var i = 0; i < count; ++i)
                {
                    if (mastery == _masteryIcons[i]?.mastery)
                    {
                        _masteryIcons[i].SetIcon(target);
                        break;
                    }
                }
            }
        }


        public void SetMasteryName(TMP_Text target, GearMastery mastery)
        {
            if (target)
            {
                target.SetText(mastery.ToString().ToUpper());
            }
        }
    }
}