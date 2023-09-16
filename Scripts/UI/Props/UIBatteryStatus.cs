using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using CustomInspector;
using UniRx;
using UniRx.Triggers;


namespace Studio.Game
{
    public class UIBatteryStatus : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField, Preview(Size.small)]
        private Sprite _charging, _low, _medium, _high;
        [SerializeField]
        private TMP_Text _level;
        [SerializeField]
        private float _checkCycleSecond = 3f;


        private void Start()
        {
            this.UpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromSeconds(_checkCycleSecond))
                .Subscribe(_ => SetStatus())
                .AddTo(this);
        }


        private void SetStatus()
        {
            var level = 0f;

            try
            {
                if (SystemInfo.batteryStatus is BatteryStatus.Charging)
                {
                    SetSprite(_charging);
                }
                else
                {
                    level = SystemInfo.batteryLevel;

                    if (0.95f <= level)
                    {
                        SetSprite(_high);
                    }
                    else if (0.5f <= level)
                    {
                        SetSprite(_medium);
                    }
                    else
                    {
                        SetSprite(_low);
                    }
                }
            }
            catch
            {
                SetSprite(_low);
            }
            finally
            {
                SetText(level);
            }
        }


        private void SetSprite(Sprite sprite)
        {
            if (_icon)
            {
                _icon.sprite = sprite;
                _icon.enabled = sprite;
                _icon.SetNativeSize();
            }
        }


        private void SetText(float level)
        {
            if (_level)
            {
                if (0f < level)
                {
                    _level.SetText(1f == level ? "100%" : $"{level * 100f:00}%");
                    _level.enabled = true;
                }
                else
                {
                    _level.enabled = false;
                }
            }
        }
    }
}