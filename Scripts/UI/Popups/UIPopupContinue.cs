using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    [Popup(nameof(UIPopupContinue), "Popups")]
    public class UIPopupContinue : UIPopup
    {
        #region model
        public class Model : IPopupModel
        {
            public int costGem { get; private set; }
            public string costGemTxt { get; private set; }
            public Action onContinue { get; private set; }

            public Model(int costGem, Action onContinue)
            {
                this.costGem = costGem;
                costGemTxt = $"<size=\"48\">X</size> {this.costGem}";
                this.onContinue = onContinue;
            }
        }
        #endregion

        [SerializeField]
        private GameObject _fx;
        [SerializeField]
        private TMP_Text _continueGemAmount;
        [SerializeField]
        private Button _giveup, _continue;


        private void OnValidate() => SetActiveFX(false);


        protected override void Start()
        {
            base.Start();

            _giveup.SetClickEvent(OnGiveUp);
            _continue.SetClickEvent(OnContinue);
        }


        protected override void ApplyModel()
        {
            base.ApplyModel();

            if (_model is Model model)
            {
                if (_continueGemAmount)
                {
                    _continueGemAmount.SetText(model.costGemTxt);
                }
            }
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();
            SetActiveFX(true);
        }


        private void SetActiveFX(bool active)
        {
            if (_fx)
            {
                _fx.SetActive(active);
            }
        }


        private void OnGiveUp()
        {
            Popup<UIPopupConfirm>.instance.OnPopup(new UIPopupConfirm.Model(
                "도전 포기",
                "현재의 도전을 포기하시겠습니까?",
                () =>
                {
                    SceneContext.GotoContent(GameScene.HOME);
                    OnClose();
                }));
        }


        private void OnContinue()
        {
            if (_model is Model model)
            {
                if (Global<User>.instance.gem.Value >= model.costGem)
                {
                    Popup<UIPopupConfirm>.instance.OnPopup(new UIPopupConfirm.Model(
                        "이어하기",
                        $"보석 {model.costGem}개를 사용하여 이어서 도전 하시겠습니까?" +
                        $"\n(모든 영웅이 70%의 HP로 회복, 부활 합니다.)",
                        () =>
                        {
                            Global<User>.instance.gem.Value -= model.costGem;
                            model.onContinue?.Invoke();
                            OnClose();
                        }));
                }
                else
                {
                    Popup<UIPopupWarning>.instance.OnPopup(new UIPopupWarning.Model("보석 부족", "보유한 보석이 부족 합니다.", null));
                }
            }
        }
    }
}