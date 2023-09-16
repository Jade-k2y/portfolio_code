using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UIMenuShop : UIMenu
    {
        #region tab
        [Serializable]
        public class Tab
        {
            [SerializeField]
            private CanvasGroup _canvasGroup;
            [SerializeField]
            private Toggle _toggle;
            [SerializeField]
            private Image _image;
            [SerializeField]
            private TMP_Text _text;

            public bool isOn
            {
                get => _toggle ? _toggle.isOn : false;
                set
                {
                    if (_toggle)
                    {
                        _toggle.isOn = value;
                    }
                }
            }

            public void SetActiveTab(bool active, Color color)
            {
                if (_canvasGroup)
                {
                    if (active)
                    {
                        _canvasGroup.DOFade(1f, 0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => _canvasGroup.Show());
                    }
                    else
                    {
                        _canvasGroup.DOFade(0f, 0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() => _canvasGroup.Hide());
                    }
                }

                if (_image)
                {
                    _image.color = color;
                }

                if (_text)
                {
                    _text.color = color;
                }
            }
        }
        #endregion //tab

        [SerializeField]
        private Tab _character, _gem, _gold;
        [SerializeField]
        private Color _activeColor;
        [SerializeField]
        private Color _inactiveColor;

        private EventSystem _cachedEventSystem;

        public override Menu menu => Menu.Shop;


        protected override void Start()
        {
            base.Start();

            if (_gem is not null)
            {
                _gem.isOn = true;
            }

            OnToggleGemChanged(true);
        }


        public override void OnMenuFocused(UIMenu prevMenu)
        {
        }


        public void OnToggleCharacterChanged(bool active)
        {
            if (active)
            {
                _character?.SetActiveTab(true, _activeColor);
                _gem?.SetActiveTab(false, _inactiveColor);
                _gold?.SetActiveTab(false, _inactiveColor);
            }
        }


        public void OnToggleGemChanged(bool active)
        {
            if (active)
            {
                _character?.SetActiveTab(false, _inactiveColor);
                _gem?.SetActiveTab(true, _activeColor);
                _gold?.SetActiveTab(false, _inactiveColor);
            }
        }


        public void OnToggleGoldChanged(bool active)
        {
            if (active)
            {
                _character?.SetActiveTab(false, _inactiveColor);
                _gem?.SetActiveTab(false, _inactiveColor);
                _gold?.SetActiveTab(true, _activeColor);
            }
        }


        public void OnAttractorStarted()
        {
            if (_cachedEventSystem = EventSystem.current)
            {
                _cachedEventSystem.enabled = false;
            }
        }


        public void OnAttractorFinished()
        {
            if (_cachedEventSystem)
            {
                _cachedEventSystem.enabled = true;
            }
            else if (EventSystem.current)
            {
                EventSystem.current.enabled = true;
            }
        }
    }
}