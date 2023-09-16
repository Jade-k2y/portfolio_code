using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using CustomInspector;
using System.Collections;


namespace Studio.Game
{
    public class UIStartScreen : MonoBehaviour, IStartScreen
    {
        [Serializable]
        public class Particle : IStartScreen.IParticle
        {
            [SerializeField]
            private ParticleSystem _main;
            [SerializeField]
            private ParticleSystem _glow;
            [SerializeField]
            private ParticleSystem _spread;

            ParticleSystem IStartScreen.IParticle.main => _main;
            ParticleSystem IStartScreen.IParticle.glow => _glow;
            ParticleSystem IStartScreen.IParticle.spread => _spread;
        }

        [SerializeField]
        private UIStartScreenScriptable _scriptable;
        [SerializeField, Validate(nameof(IsValidate))]
        private GameScene _gameScene;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Particle _particle;
        [SerializeField]
        private float _delay, _duration;

        public TMP_Text title => _title;
        Image IStartScreen.icon => _icon;
        IStartScreen.IParticle IStartScreen.particle => _particle;


        private void OnValidate()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }


        private void Start()
        {
            if (_scriptable)
            {
                _scriptable.SetPresenter(_gameScene, this);
            }
        }


        private bool IsValidate(GameScene scene) => scene switch
        {
            GameScene.STORY_EPISODE => true,
            GameScene.STORY_STAGE => true,
            GameScene.GEAR_STAGE => true,
            GameScene.RAID_STAGE => true,
            GameScene.GEM_STAGE => true,
            GameScene.GOLD_STAGE => true,
            GameScene.PVP_STAGE => true,
            GameScene.EVENT_STAGE => true,
            _ => false
        };


        public IEnumerator Fadeout()
        {
            if (_canvasGroup)
            {
                var complete = false;

                _canvasGroup.alpha = 1f;
                _canvasGroup.DOFade(0f, _duration)
                    .SetUpdate(true)
                    .SetDelay(_delay)
                    .OnComplete(() =>
                    {
                        _canvasGroup.SetInteractable(false);
                        _canvasGroup.SetBlocksRaycasts(false);

                        complete = true;
                    })
                    .SetLink(gameObject);

                yield return new WaitUntil(() => complete);
            }
        }
    }
}