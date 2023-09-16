using UnityEngine;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UISpeechBubble : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private TMP_Text _speech;

        private Transform _anchor;
        private Camera _camera;
        private Canvas _canvas;


        private void OnValidate() => ResetSpeech();


        private void Start()
        {
            _camera = Camera.main;
            _canvasGroup.Hide();
        }


        private void Update()
        {
            if (_anchor && _camera && _canvas)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    _canvas.transform as RectTransform,
                    RectTransformUtility.WorldToScreenPoint(_camera, _anchor.position),
                    _canvas.worldCamera,
                    out var pos))
                {
                    transform.position = pos;
                }
            }
        }


        public void SetCanvas(Canvas canvas) => _canvas = canvas;


        public void SetAnchor(Transform anchor) => _anchor = anchor;


        public void OnSpeech(string speech, TweenCallback onFinished)
        {
            ResetSpeech();

            EnableSpeechBubble(true, () =>
            {
                if (_speech)
                {
                    _speech.DOText(speech, (speech?.Length ?? 0) * 0.01f)
                        .OnComplete(() =>
                        {
                            EnableSpeechBubble(false, onFinished);
                        })
                        .SetLink(gameObject);
                }
            });
        }


        private void EnableSpeechBubble(bool enable, TweenCallback onCompleted)
        {
            if (_canvasGroup)
            {
                if (enable)
                {
                    _canvasGroup.Show();
                    onCompleted?.Invoke();
                }
                else
                {
                    _canvasGroup.DOFade(0f, GameConstant.DefaultPresentDuration)
                        .SetDelay(!enable ? 2f : 0f)
                        .OnComplete(onCompleted)
                        .SetLink(gameObject);
                }
            }
            else
            {
                onCompleted?.Invoke();
            }
        }


        private void ResetSpeech()
        {
            if (_speech)
            {
                _speech.SetText(string.Empty);
            }
        }
    }
}