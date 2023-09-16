using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UIHealthBar : MonoBehaviour, ICombatActionPresenter, IHealthBarGUI
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Image _bar, _bossIcon;
        [SerializeField]
        private TMP_Text _damage;
        [SerializeField]
        private float _duration;

        private Canvas _canvas;
        private bool _transformUpdated;

        RectTransform IHealthBarGUI.frame => transform as RectTransform;
        Graphic IHealthBarGUI.bar => _bar;
        Graphic IHealthBarGUI.bossIcon => _bossIcon;


        private void Start() => _canvasGroup.Hide();


        public UIHealthBar SetCanvas(Canvas canvas)
        {
            _canvas = canvas;

            return this;
        }


        bool ICombatActionPresenter.Initialize()
        {
            if (0f >= _duration)
            {
                _duration = 0.5f;
            }

            if (_bar)
            {
                _bar.fillAmount = 1f;
            }

            _canvasGroup.Hide();
            _transformUpdated = false;

            return true;
        }


        void ICombatActionPresenter.OnResurrection(Stats.HP hp)
        {
            if (hp is not null)
            {
                if (_bar)
                {
                    _bar.fillAmount = hp.rate;
                }
            }
        }


        void ICombatActionPresenter.OnTakeAction(ICombatActionModel model, Vector3 anchor, Stats.HP hp)
        {
            if (!_transformUpdated && _canvas)
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    _canvas.transform as RectTransform,
                    RectTransformUtility.WorldToScreenPoint(Camera.main, anchor),
                    _canvas.worldCamera,
                    out var pos))
                {
                    transform.position = pos;
                }
                _transformUpdated = true;
            }

            if (model?.actionType is CombatActionType.Attack)
            {
                _canvasGroup.Show();

                if (_damage)
                {
                    _damage.SetText(Mathf.Abs(hp.changed).ToString());
                    _damage.transform.DOPunchScale(model.isCritical ? Vector3.one * 2f : Vector3.one, _duration)
                        .SetRelative(true)
                        .OnComplete(() => _canvasGroup.Hide())
                        .SetLink(gameObject);
                }

                if (_bar)
                {
                    _bar.DOFillAmount(hp?.rate ?? 0f, _duration * 0.5f)
                        .SetLink(gameObject);
                }
            }
        }


        void IHealthBarGUI.SetTextColor(Color color)
        {
            if (_damage)
            {
                _damage.color = color;
            }
        }
    }
}