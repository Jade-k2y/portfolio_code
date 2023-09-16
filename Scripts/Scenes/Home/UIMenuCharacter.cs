using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UIMenuCharacter : UIMenu
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
                    var current = EventSystem.current;

                    if (current)
                    {
                        current.enabled = false;
                    }

                    if (active)
                    {
                        _canvasGroup.DOFade(1f, 0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                if (current)
                                {
                                    current.enabled = true;
                                }

                                _canvasGroup.Show();
                            });
                    }
                    else
                    {
                        _canvasGroup.DOFade(0f, 0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                if (current)
                                {
                                    current.enabled = true;
                                }

                                _canvasGroup.Hide();
                            });
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
        private Tab _characterList, _teamList, _inventory;
        [SerializeField]
        private Color _activeColor; //F6E19C
        [SerializeField]
        private Color _inactiveColor; //BEB5B6

        private readonly Queue<ITutorial> _tutorials = new();

        public override Menu menu => Menu.Character;


        private void Awake()
        {
            var tutorials = GetComponents<ITutorial>()
                ?.Where(x => !x.isDone && !x.useTrigger)
                ?.OrderBy(x => x.order);

            foreach (var tutorial in tutorials)
            {
                _tutorials.Enqueue(tutorial);
            }
        }


        protected override void Start()
        {
            base.Start();

            if (_teamList is not null)
            {
                _teamList.isOn = true;
            }

            OnToggleTeamListChanged(true);
        }


        public override void OnMenuFocused(UIMenu prevMenu)
        {
            if (0 < _tutorials.Count)
            {
                StartCoroutine(RunTutorials());
            }
        }


        private IEnumerator RunTutorials()
        {
            yield return new WaitForEndOfFrame();

            while (0 < _tutorials.Count)
            {
                yield return _tutorials.Dequeue().Play();
            }
        }


        public void OnToggleCharacterListChanged(bool active)
        {
            if (active)
            {
                _characterList?.SetActiveTab(true, _activeColor);
                _teamList?.SetActiveTab(false, _inactiveColor);
                _inventory?.SetActiveTab(false, _inactiveColor);
            }
        }


        public void OnToggleTeamListChanged(bool active)
        {
            if (active)
            {
                _characterList?.SetActiveTab(false, _inactiveColor);
                _teamList?.SetActiveTab(true, _activeColor);
                _inventory?.SetActiveTab(false, _inactiveColor);
            }
        }


        public void OnToggleInventoryChanged(bool active)
        {
            if (active)
            {
                _characterList?.SetActiveTab(false, _inactiveColor);
                _teamList?.SetActiveTab(false, _inactiveColor);
                _inventory?.SetActiveTab(true, _activeColor);
            }
        }
    }
}