using UnityEngine;
using UnityEngine.UI;


namespace Studio.Game
{
    [Popup(nameof(UIPopupSetting), "Popups")]
    public class UIPopupSetting : UIPopup
    {
        [SerializeField]
        private Toggle _tutorial;
        [SerializeField]
        private Button _close, _reset;

        private bool _isStartToggle;


        protected override void Start()
        {
            base.Start();

            if (_tutorial)
            {
                _tutorial.isOn = Global<User>.instance.isShowTutorial;
                _tutorial.onValueChanged.RemoveAllListeners();
                _tutorial.onValueChanged.AddListener(OnTutorialToggleChanged);
            }

            _close.SetClickEvent(OnClose, false);
            _reset.SetClickEvent(OnResetAccount);
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();

            if (_tutorial)
            {
                _isStartToggle = _tutorial.isOn;
            }
        }


        private void OnTutorialToggleChanged(bool isOn)
        {
            if (isOn)
            {
                if (!_isStartToggle)
                {
                    Popup<UIPopupConfirm>.instance.OnPopup(
                        new UIPopupConfirm.Model(
                            "재실행 확인",
                            "튜토리얼 실행을 위해 게임이 다시 실행 됩니다.",
                            () =>
                            {
                                Global<User>.instance.isShowTutorial = true;
                                Global<User>.instance.SaveData();
                                SceneContext.GotoTitle();
                            },
                            () =>
                            {
                                if (_tutorial)
                                {
                                    _tutorial.isOn = false;
                                }
                            }));
                }
                else
                {
                    Global<User>.instance.isShowTutorial = true;
                }
            }
            else
            {
                Global<User>.instance.isShowTutorial = false;
            }
        }


        private void OnResetAccount()
        {
            Popup<UIPopupConfirm>.instance.OnPopup(
                new UIPopupConfirm.Model(
                    "계정 초기화 확인", 
                    "현재 계정을 초기화 하시겠습니까?" +
                    "\n게임이 다시 실행 됩니다.",
                    () =>
                    {
                        UserManager.Reset(() =>
                        {
                            OnClose();
                            SceneContext.GotoTitle();
                        });
                    }));
        }
    }
}