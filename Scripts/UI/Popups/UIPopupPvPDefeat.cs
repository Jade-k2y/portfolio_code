using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    [Popup(nameof(UIPopupPvPDefeat), "Popups")]
    public class UIPopupPvPDefeat : UIPopup
    {
        [SerializeField]
        private GameObject _fx;
        [SerializeField]
        private TMP_Text _score;
        [SerializeField]
        private Button _exit;


        private void OnValidate() => SetActiveFX(false);


        protected override void Start()
        {
            base.Start();

            _exit.SetClickEvent(() =>
            {
                SceneContext.GotoContent(GameScene.HOME);
                OnClose();
            });
        }


        protected override void OnPopupEnabled()
        {
            base.OnPopupEnabled();
            SetActiveFX(true);

            if (_score)
            {
                _score.DOCounter(1750, (1750 - 108), 1f)
                    .SetDelay(1f)
                    .SetLink(gameObject);
            }
        }


        private void SetActiveFX(bool active)
        {
            if (_fx)
            {
                _fx.SetActive(active);
            }
        }
    }
}