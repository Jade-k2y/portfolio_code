using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;


namespace Studio.Game
{
    public class UIShopItem : MonoBehaviour
    {
        [SerializeField]
        private GameAsset _asset;
        [SerializeField]
        private Transform _item;
        [SerializeField]
        private int _amount;
        [SerializeField]
        private TMP_Text _product, _price;
        [SerializeField]
        private Button _buy;
        [SerializeField]
        private UIAttractor _attractor;

        private readonly Stack<int> _additionals = new();
        private float _rate;
        private string _purchase;


        private void Start()
        {
            if (_product)
            {
                _purchase = $"[{_product.text}] {_amount}개를 구매 하시겠습니까?";
            }
            else
            {
                _purchase = $"[{_asset}] {_amount}개를 구매 하시겠습니까?";
            }

            _buy.SetClickEvent(() =>
            {
                Popup<UIPopupConfirm>.instance.OnPopup(new UIPopupConfirm.Model("구매 확인", _purchase, OnPurchased));
            });
        }


        private void OnPurchased()
        {
            _additionals.Clear();

            if (_attractor)
            {
                if (_item)
                {
                    _attractor.transform.position = _item.position;
                }

                _attractor.SetRateOverTime(GetAmountRate());
                _attractor.SetPieceArrived(OnAdditionalAsset);
                _attractor.Play();
            }
        }


        private float GetAmountRate()
        {
            if (0f >= _rate)
            {
                var count = 6;
                var sibling = transform.GetSiblingIndex() + 1;

                if (transform.parent)
                {
                    count = transform.parent.childCount;
                }

                _rate = (float)sibling / count;
            }

            return _rate;
        }


        private void OnAdditionalAsset()
        {
            if (0 >= _additionals.Count)
            {
                var count = _attractor.count;
                var add = _amount / count;

                while (0 != count)
                {
                    if (1 == count--)
                    {
                        _additionals.Push(_amount - _additionals.Sum());
                    }
                    else
                    {
                        _additionals.Push(add);
                    }
                }
            }

            if (0 < _additionals.Count)
            {
                if (_asset is GameAsset.Gem)
                {
                    Global<User>.instance.gem.Value += _additionals.Pop();
                }
                else if (_asset is GameAsset.Gold)
                {
                    Global<User>.instance.gold.Value += _additionals.Pop();
                }
            }
        }
    }
}