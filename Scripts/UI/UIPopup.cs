using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using DG.Tweening;
using CustomInspector;


namespace Studio.Game
{
    public interface IPopupModel { }


    [RequireComponent(typeof(Canvas))]
    public abstract class UIPopup : MonoBehaviour
    {
        [Serializable]
        public class LayerCombine
        {
            [SerializeField]
            private RectTransform[] _tops, _bottoms, _lefts, _rights;

            public void OnCombine(TweenCallback onEnabled)
            {
                var tweener = default(Tween);

                DoTweening(_tops, Vector2.up, ref tweener);
                DoTweening(_bottoms, Vector2.down, ref tweener);
                DoTweening(_lefts, Vector2.left, ref tweener);
                DoTweening(_rights, Vector2.right, ref tweener);

                if (tweener is not null)
                {
                    tweener.OnComplete(onEnabled);
                }
                else
                {
                    onEnabled?.Invoke();
                }
            }

            private void DoTweening(RectTransform[] rects, Vector2 dir, ref Tween tweener)
            {
                var count = rects?.Length;

                for (var i = 0; i < count; ++i)
                {
                    if (rects[i])
                    {
                        var start = rects[i].anchoredPosition;
                        var end = dir * GameConstant.LayerPresentDistance + start;

                        rects[i].localPosition = end;

                        var tween = rects[i].DOAnchorPos(start, GameConstant.DefaultPresentDuration)
                            .SetUpdate(true)
                            .SetAutoKill(true)
                            .SetLink(rects[i].gameObject);

                        tweener ??= tween;
                    }
                }
            }
        }

        public enum Presentation
        {
            None,
            Fade,
            Scale,
            Combine
        }

        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Presentation _presentation;
        [SerializeField, ShowIf(nameof(IsShowLayerCombine))]
        private LayerCombine _layerCombine;
        [SerializeField]
        private bool _closeOnBackground;
        [SerializeField, ShowIf(nameof(_closeOnBackground))]
        protected Button _background;

        protected IPopupModel _model;

        private bool _initialized;
        private Action _onDisabled;


        public bool IsShowLayerCombine() => _presentation is Presentation.Combine;


        protected virtual void Start() => Initialize();


        protected virtual void StartPopup() { }


        private void Initialize()
        {
            if (!_initialized)
            {
                StartPopup();

                if (_canvas)
                {
                    _canvas.enabled = false;
                    _canvas.overrideSorting = true;
                    _canvas.sortingLayerName = "UI";
                    _canvas.sortingOrder = 10;
                }

                if (_canvasGroup)
                {
                    _canvasGroup.alpha = _presentation is Presentation.Fade ? 0f : 1f;
                    _canvasGroup.transform.localScale = _presentation is Presentation.Scale ? Vector3.zero : Vector3.one;
                }

                if (_closeOnBackground && _background)
                {
                    _background.SetClickEvent(OnClose);
                }

                var attribute = GetType().GetCustomAttribute<PopupAttribute>();

                if (attribute?.destroyOnDisabled ?? false)
                {
                    _onDisabled = () =>
                    {
                        DestroyImmediate(gameObject);
                        Resources.UnloadUnusedAssets();
                    };
                }

                _initialized = true;
            }
        }


        public virtual void OnPopup() => SetActivePopup(true);

        public virtual void OnPopup(IPopupModel model)
        {
            if ((_model = model) is not null)
            {
                ApplyModel();
            }

            SetActivePopup(true);
        }

        protected virtual void ApplyModel() { }

        public virtual void OnClose() => SetActivePopup(false);

        protected virtual void OnPopupEnabled() { }

        protected virtual void OnPopupDisabled() => _onDisabled?.Invoke();

        protected virtual void SetActivePopup(bool active)
        {
            Initialize();

            if (active)
            {
                SetCanvasActive(active);
                transform.SetAsLastSibling();
            }
            
            if (_canvasGroup)
            {
                switch (_presentation)
                {
                    case Presentation.None:
                        {
                            _canvasGroup.SetAlpha(active ? 1f : 0f);

                            if (active)
                            {
                                OnPopupEnabled();
                            }
                            else
                            {
                                OnPopupDisabled();
                            }
                        }
                        break;

                    case Presentation.Fade:
                        {
                            OnFadePopup(active);
                        }
                        break;

                    case Presentation.Scale:
                        {
                            OnScalePopup(active);
                        }
                        break;

                    case Presentation.Combine:
                        {
                            if (active)
                            {
                                _canvasGroup.SetAlpha(1f);

                                if (_layerCombine is not null)
                                {
                                    _layerCombine.OnCombine(OnPopupEnabled);
                                }
                                else
                                {
                                    OnPopupEnabled();
                                }
                            }
                            else
                            {
                                OnFadePopup(false);
                            }
                        }
                        break;
                }
            }
            else
            {
                gameObject.SetActive(active);
                
                if (active)
                {
                    OnPopupEnabled();
                }
                else
                {
                    OnPopupDisabled();
                }
            }
        }


        private void OnScalePopup(bool active)
        {
            if (_canvasGroup)
            {
                _canvasGroup.transform.DOScale(active ? Vector3.one : Vector3.zero, GameConstant.DefaultPresentDuration)
                    .SetUpdate(true)
                    .SetEase(Ease.OutExpo)
                    .SetAutoKill(true)
                    .OnComplete(() =>
                    {
                        if (active)
                        {
                            OnPopupEnabled();
                        }
                        else
                        {
                            SetCanvasActive(active);
                            OnPopupDisabled();
                        }
                    })
                    .SetLink(gameObject);
            }
        }


        private void OnFadePopup(bool active)
        {
            if (_canvasGroup)
            {
                _canvasGroup.DOFade(active ? 1f : 0f, GameConstant.DefaultPresentDuration)
                    .SetUpdate(true)
                    .SetAutoKill(true)
                    .OnComplete(() =>
                    {
                        if (active)
                        {
                            OnPopupEnabled();
                        }
                        else
                        {
                            SetCanvasActive(active);
                            OnPopupDisabled();
                        }
                    })
                    .SetLink(gameObject);
            }
        }


        private void SetCanvasActive(bool active)
        {
            if (_canvas)
            {
                _canvas.enabled = active;
            }
        }
    }
}