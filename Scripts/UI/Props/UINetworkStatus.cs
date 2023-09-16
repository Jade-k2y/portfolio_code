using UnityEngine;
using UnityEngine.UI;
using System;
using CustomInspector;
using UniRx;
using UniRx.Triggers;


namespace Studio.Game
{
    public class UINetworkStatus : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField, Preview(Size.small)]
        private Sprite _disconnected, _dataNetwork, _localAreaNetwork;
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
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    SetSprite(_disconnected);
                    break;

                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    SetSprite(_dataNetwork);
                    break;

                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    SetSprite(_localAreaNetwork);
                    break;
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
    }
}