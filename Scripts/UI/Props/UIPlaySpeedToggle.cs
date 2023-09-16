using UnityEngine;
using UnityEngine.UI;
using CustomInspector;


namespace Studio.Game
{
    [RequireComponent(typeof(Button))]
    public class UIPlaySpeedToggle : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Sprite _1xSpeed, _2xSpeed;
        [SerializeField, ReadOnly]
        private bool _is2xSpeed;
        [SerializeField]
        private Button _button;


        private void OnDestroy() => GameScriptable.SetTimeSclae(false);


        private void Start()
        {
            _button.SetClickEvent(() =>
            {
                GameScriptable.SetTimeSclae(_is2xSpeed = !_is2xSpeed);
                SetImageSprite();
            });

            SetImageSprite();
        }


        private void SetImageSprite()
        {
            if (_icon)
            {
                _icon.sprite = !_is2xSpeed ? _1xSpeed : _2xSpeed;
                _icon.SetNativeSize();
            }
        }
    }
}