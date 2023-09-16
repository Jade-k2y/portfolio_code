using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UIStoryEpisode : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Transform _body;
        [SerializeField]
        private UIStartScreen _screen;
        [SerializeField]
        private UISpeechBubble _speechAsset;
        [SerializeField]
        private Image _blackScreen;
        [SerializeField]
        private TMP_Text _narration;

        private readonly Dictionary<int, UISpeechBubble> _speeches = new();


        private void OnDestroy()
        {
            foreach (var speech in _speeches.Values)
            {
                if (speech)
                {
                    Destroy(speech.gameObject);
                }
            }

            _speeches.Clear();
        }


        public IEnumerator WaitForStart()
        {
            if (_screen)
            {
                yield return _screen.Fadeout();
            }
        }


        public void FadeOutScreen(TweenCallback onFinished)
        {
            if (_blackScreen)
            {
                _blackScreen.DOColor(Color.black, 1f)
                    .OnComplete(onFinished)
                    .SetLink(gameObject);
            }
            else
            {
                onFinished?.Invoke();
            }
        }


        public void OnNarration(string message, TweenCallback onFinished)
        {
            if (_narration)
            {
                _narration.DOText(message, 1f)
                    .OnComplete(() =>
                    {
                        OnCompleteNarration(onFinished);
                    })
                    .SetLink(gameObject);
            }
        }


        private void OnCompleteNarration(TweenCallback onFinished)
        {
            if (_narration)
            {
                _narration.DOFade(0f, 1f)
                    .SetDelay(1f)
                    .OnComplete(onFinished)
                    .SetLink(gameObject);
            }
        }


        public void GenerateSpeech(int ownerIdx, Transform anchor)
        {
            if (!_speeches.ContainsKey(ownerIdx) && _speechAsset)
            {
                var speech = Instantiate(_speechAsset, _body);

                speech.SetCanvas(_canvas);
                speech.SetAnchor(anchor);

                _speeches[ownerIdx] = speech;
            }
        }


        public void OnSpeech(int ownerIdx, string message, TweenCallback onFinished = null)
        {
            if (_speeches.ContainsKey(ownerIdx))
            {
                _speeches[ownerIdx].OnSpeech(message, onFinished);
            }
        }
    }
}