using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UniRx;
using DanielLochner.Assets.SimpleScrollSnap;


namespace Studio.Game
{
    public class UISceneHome : UIScene
    {
        #region drag sensitive
        [Serializable]
        public class DragSensitive : IDisposable
        {
            [SerializeField]
            private float _inchToCM = 2.54f, _dragThresholdCM = 0.5f;
            private int _originPixelDragThreshold;

            public void SetDragThreshold()
            {
                var eventSystem = EventSystem.current;

                if (eventSystem)
                {
                    _originPixelDragThreshold = eventSystem.pixelDragThreshold;
                    eventSystem.pixelDragThreshold = (int)(_dragThresholdCM * Screen.dpi / _inchToCM);
                }
            }

            public void Dispose()
            {
                var eventSystem = EventSystem.current;

                if (eventSystem)
                {
                    eventSystem.pixelDragThreshold = _originPixelDragThreshold;
                }
            }
        }
        #endregion //drag sensitive

        #region menu element
        [Serializable]
        public class MenuElement
        {
            [SerializeField]
            private UIMenu _menu;
            [SerializeField]
            private Transform _icon;
            [SerializeField]
            private TMP_Text _text;

            public UIMenu menu => _menu;
            public Transform icon => _icon;
            public TMP_Text text => _text;
        }
        #endregion //menu element

        #region asset
        [Serializable]
        public class Asset
        {
            [SerializeField]
            private TMP_Text _amount;
            [SerializeField]
            private Button _button;

            public TMP_Text amount => _amount;
            public Button button => _button;
        }
        #endregion //asset

        [SerializeField]
        private SimpleScrollSnap _scrollSnap;
        [SerializeField]
        private DragSensitive _dragSensitive;
        [SerializeField]
        private Asset _assetGem, _assetGold;
        [SerializeField]
        private Button _setting;
        [SerializeField]
        private MenuElement[] _menu;
        [SerializeField]
        private float _focusScale = 1.25f;
        [SerializeField]
        private float _focusDuration = 0.15f;
        [SerializeField]
        private Color _focusColor = new Color32(246, 225, 156, 255);


        private void OnDestroy()
        {
            _dragSensitive?.Dispose();
        }


        protected override void Start()
        {
            base.Start();

            _dragSensitive?.SetDragThreshold();

            Global<User>.instance.gem.Subscribe(gem =>
            {
                if (_assetGem?.amount)
                {
                    _assetGem.amount.SetText($"{gem:#,###}");
                }
            }).AddTo(this);

            Global<User>.instance.gold.Subscribe(gold =>
            {
                if (_assetGold?.amount)
                {
                    _assetGold.amount.SetText($"{gold:#,###}");
                }
            }).AddTo(this);

            if (_assetGem?.button)
            {
                _assetGem.button.SetClickEvent(() => OnMoveMenu(UIMenu.Menu.Shop), false);
            }

            if (_assetGold?.button)
            {
                _assetGold.button.SetClickEvent(() => OnMoveMenu(UIMenu.Menu.Shop), false);
            }

            _setting.SetClickEvent(() =>
            {
                Popup<UIPopupSetting>.instance.OnPopup();
            });

            if (_scrollSnap)
            {
                OnPanelCentered(_scrollSnap.StartingPanel, _scrollSnap.StartingPanel);
            }
        }


        public void OnMoveMenu(UIMenu.Menu menu)
        {
            if (_scrollSnap)
            {
                _scrollSnap.GoToPanel((int)menu);
            }
        }


        public void OnPanelCentered(int centeredPanel, int selectedPanel)
        {
            var count = _menu?.Length;

            for (var i = 0; i < count; ++i)
            {
                var element = GetMenuElement(i);

                if (i == centeredPanel)
                {
                    if (element?.menu)
                    {
                        element.menu.OnMenuFocused(GetMenuElement(centeredPanel)?.menu);
                    }

                    if (element?.icon)
                    {
                        element.icon.DOScale(_focusScale, _focusDuration).SetUpdate(true);
                    }

                    if (element?.text)
                    {
                        element.text.color = _focusColor;
                    }
                }
                else
                {
                    if (element?.icon)
                    {
                        element.icon.localScale = Vector3.one;
                    }

                    if (element?.text)
                    {
                        element.text.color = Color.white;
                    }
                }
            }
        }


        private MenuElement GetMenuElement(int index)
        {
            if (0 <= index && index < _menu?.Length)
            {
                return _menu[index];
            }

            return default;
        }
    }
}