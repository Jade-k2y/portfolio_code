using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    [Popup(nameof(UIPopupWarning), "Popups")]
    public class UIPopupWarning : UIPopup
    {
        #region model
        public class Model : IPopupModel
        {
            public string title { get; private set; }
            public string message { get; private set; }
            public Action onConfirm { get; private set; }

            public Model(string title, string message, Action onConfirm)
            {
                this.title = title;
                this.message = message;
                this.onConfirm = onConfirm;
            }
        }
        #endregion

        [SerializeField]
        private TMP_Text _title, _message;
        [SerializeField]
        private Button _confirm;


        protected override void Start()
        {
            base.Start();

            _confirm.SetClickEvent(OnClickConfirm);
        }


        protected override void ApplyModel()
        {
            base.ApplyModel();

            if (_model is Model model)
            {
                if (_title)
                {
                    _title.SetText(model.title);
                }

                if (_message)
                {
                    _message.SetText(model.message);
                }
            }
        }


        private void OnClickConfirm()
        {
            OnClose();

            if (_model is Model model)
            {
                model.onConfirm?.Invoke();
            }
        }
    }
}