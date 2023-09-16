using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    [Popup(nameof(UIPopupStagePause), "Popups", destroyOnDisabled: true)]
    public class UIPopupStagePause : UIPopup
    {
        public class Model : IPopupModel
        {
            public GameScene scene { get; private set; }
            public Action<TMP_Text> onTitleSetter { get; private set; }

            public Model(GameScene scene, Action<TMP_Text> onTitleSetter)
            {
                this.scene = scene;
                this.onTitleSetter = onTitleSetter;
            }
        }


        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private Button _setting, _continue, _restart, _giveup;
        [SerializeField]
        private Image _disable;

        private float _restoreTimeScale;


        protected override void Start()
        {
            base.Start();

            _background.SetClickEvent(() =>
            {
                Time.timeScale = _restoreTimeScale;
                OnClose();
            }, false);
            _setting.SetClickEvent(() =>
            {
                Popup<UIPopupSetting>.instance.OnPopup();
            });
            _continue.SetClickEvent(() =>
            {
                Time.timeScale = _restoreTimeScale;
                OnClose();
            });
            _restart.SetClickEvent(() =>
            {
                Time.timeScale = 1f;
                SceneContext.GotoStory();
                OnClose();
            });
            _giveup.SetClickEvent(() =>
            {
                Time.timeScale = 1f;
                SceneContext.GotoContent(GameScene.HOME);
                OnClose();
            });
        }


        protected override void ApplyModel()
        {
            base.ApplyModel();

            if (_model is Model model)
            {
                model.onTitleSetter?.Invoke(_title);

                var enable = model.scene is GameScene.STORY_STAGE;

                if (_restart)
                {
                    _restart.interactable = enable;
                }

                if (_disable)
                {
                    _disable.enabled = !enable;
                }
            }
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();

            _restoreTimeScale = Time.timeScale;

            Time.timeScale = 0f;
        }
    }
}