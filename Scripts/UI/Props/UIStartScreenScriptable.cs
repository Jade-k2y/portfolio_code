using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using CustomInspector;


namespace Studio.Game
{
    public interface IStartScreen
    {
        public interface IParticle
        {
            public ParticleSystem main { get; }
            public ParticleSystem glow { get; }
            public ParticleSystem spread { get; }
        }

        TMP_Text title { get; }
        Image icon { get; }
        IParticle particle { get; }
    }


    [CreateAssetMenu(menuName = "Crypto TF/Game/UI/Start Screen")]
    public class UIStartScreenScriptable : ScriptableObject
    {
        [Serializable]
        public class MinMaxColor
        {
            public Color min;
            public Color max;

            public void SetColor(ParticleSystem partice)
            {
                if (partice)
                {
                    var main = partice.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(min, max);
                }
            }
        }

        [Serializable]
        public class ParticleColor
        {
            public MinMaxColor main;
            public MinMaxColor glow;
            public MinMaxColor spread;
        }

        [Serializable]
        public class PlayScreen
        {
            [HideInInspector]
            public string name;
            [SerializeField, Validate(nameof(IsValidate))]
            private GameScene _scene;
            [SerializeField, Preview]
            private Sprite _icon;
            [SerializeField, Unfold]
            private ParticleColor _fxColor;

            public GameScene scene => _scene;

            public void OnValidate() => name = _scene.ToString();

            public void SetPresenter(IStartScreen screen)
            {
                if (screen is not null)
                {
                    if (screen.icon)
                    {
                        screen.icon.enabled = screen.icon.sprite = _icon;
                    }

                    if (_fxColor is not null && screen.particle is not null)
                    {
                        _fxColor.main?.SetColor(screen.particle.main);
                        _fxColor.glow?.SetColor(screen.particle.glow);
                        _fxColor.spread?.SetColor(screen.particle.spread);
                    }
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
        }

        [SerializeField]
        private PlayScreen[] _screens;


        private void OnValidate()
        {
            var count = _screens?.Length;

            for (var i = 0; i < count; ++i)
            {
                _screens[i]?.OnValidate();
            }
        }


        public void SetPresenter(GameScene gameScene, IStartScreen screen)
        {
            var count = _screens?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (gameScene == _screens[i]?.scene)
                {
                    _screens[i].SetPresenter(screen);
                    break;
                }
            }
        }
    }
}