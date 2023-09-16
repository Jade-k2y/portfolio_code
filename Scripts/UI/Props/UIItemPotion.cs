using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UniRx;


namespace Studio.Game
{
    public class UIItemPotion : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Graphic _icon;
        [SerializeField]
        private TMP_Text _amount;
        [SerializeField]
        private Image _restrictIcon;
        [SerializeField]
        private UnityEvent<bool> _onToggleItem;

        private bool _restrict = true;


        private void Start()
        {
            Global<User>.instance.itemPotion.Subscribe(OnUseItem).AddTo(this);

            if (_toggle)
            {
                _toggle.interactable = !_restrict;
            }

            if (_amount)
            {
                _amount.color = _restrict ? GameConstant.DisableTextColor : Color.white;
            }
        }


        public void SetActive(bool active)
        {
            if (_canvasGroup)
            {
                if (active)
                {
                    _canvasGroup.Show();
                }
                else
                {
                    _canvasGroup.Hide();
                }
            }
            else
            {
                gameObject.SetActive(active);
            }
        }


        private void OnUseItem(int amount)
        {
            amount = 0 > amount ? 0 : amount;

            if (_toggle)
            {
                if (0 >= amount)
                {
                    if (_toggle.isOn)
                    {
                        _toggle.isOn = false;
                    }

                    _toggle.enabled = false;
                }
                else
                {
                    _toggle.enabled = true;
                }
            }

            if (_icon)
            {
                _icon.color = 0 < amount ? Color.white : Color.gray;
            }

            if (_amount)
            {
                _amount.SetText(amount.ToString());
                _amount.color = 0 < amount ? Color.white : GameConstant.EmptyTextColor;
            }
        }


        public void OnToggleChanged(bool isOn) => _onToggleItem?.Invoke(isOn);
    }
}