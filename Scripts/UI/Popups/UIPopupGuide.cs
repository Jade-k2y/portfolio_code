using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using UniRx;


namespace Studio.Game
{
    [Popup(nameof(UIPopupGuide), "Popups", keepAlive: true)]
    public class UIPopupGuide : UIPopup
    {
        #region model
        public class Model : IPopupModel, IDisposable
        {
            private TutorialGuide _guide;
            private Tween _tween;
            private Action _onDisposed;

            public bool isHoldGame => _guide?.hold ?? false;

            public Model(TutorialGuide guide) => _guide = guide;

            public Model(TutorialGuide guide, Action onDispose)
            {
                _guide = guide;
                _onDisposed = onDispose;
            }

            public void Dispose()
            {
                _tween?.Kill();
                _onDisposed?.Invoke();
            }

            public void SetView(UIPopupGuide view, Action onCallback)
            {
                if (view && _guide is not null)
                {
                    if (view._anchor)
                    {
                        view._anchor.anchoredPosition = new Vector2(0f, _guide.y);
                    }

                    if (view._circle)
                    {
                        view._circle.gameObject.SetActive(false);
                    }
                    
                    if (view._guide)
                    {
                        view._guide.DOText(_guide.message, (_guide.message?.Length ?? 0) * 0.02f)
                            .SetUpdate(true)
                            .SetDelay(GameConstant.DefaultPresentDuration)
                            .SetLink(view.gameObject)
                            .OnComplete(() =>
                            {
                                if (view._circle)
                                {
                                    view._circle.gameObject.SetActive(_guide.isFocus);
                                    view._circle.transform.position = _guide.focusTransform ? _guide.focusTransform.position : _guide.focus;
                                    view._circle.transform.localScale *= _guide.rate;

                                    if (_guide.tweening)
                                    {
                                        _tween = view._circle.transform.DOMove(_guide.focusMoveTransform ? _guide.focusMoveTransform.position : _guide.focusMove, _guide.duration)
                                            .SetUpdate(true)
                                            .SetLoops(-1, LoopType.Restart)
                                            .SetLink(view.gameObject);
                                    }
                                }

                                onCallback?.Invoke();
                            });
                    }
                }
            }
        }
        #endregion

        [SerializeField]
        private RectTransform _anchor;
        [SerializeField]
        private ParticleSystem _circle;
        [SerializeField]
        private Vector3 _scale;
        [SerializeField]
        private TMP_Text _guide;
        [SerializeField]
        private Image _next;
        [SerializeField, Min(1f)]
        private float _nextDelay;

        private Subject<Unit> _delay = new();
        private float _restoreTimeScale;
        private bool _isShowTutorial;


        private void OnValidate()
        {
            if (_guide)
            {
                _guide.SetText(" ");
            }
        }


        protected override void Start()
        {
            base.Start();

            _background.SetClickEvent(() => OnClose(), false);
        }


        protected override void SetActivePopup(bool active)
        {
            if (active)
            {
                if (_circle)
                {
                    _circle.transform.localScale = _scale;
                }

                SetActiveNext(false);

                (_model as Model).SetView(this, () =>
                {
                    SetActiveNext(true);
                });

                _delay.Delay(TimeSpan.FromSeconds(_nextDelay), Scheduler.MainThreadIgnoreTimeScale)
                    .First()
                    .Subscribe(_ => SetActiveNext(true));
                _delay.OnNext(Unit.Default);
            }
            else
            {
                if (_guide)
                {
                    _guide.SetText(" ");
                }
            }

            base.SetActivePopup(active);
        }


        private void SetActiveNext(bool active)
        {
            if (_next)
            {
                _next.enabled = active;
            }

            if (_background)
            {
                _background.interactable = active;
            }
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();

            if ((_model as Model)?.isHoldGame ?? false)
            {
                _restoreTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }


        protected override void OnPopupDisabled()
        {
            base.OnPopupDisabled();

            if (_model is Model model)
            {
                if (model.isHoldGame)
                {
                    Time.timeScale = _restoreTimeScale;
                }

                model.Dispose();
            }
        }
    }
}