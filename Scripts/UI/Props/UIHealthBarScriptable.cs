using UnityEngine;
using UnityEngine.UI;
using CustomInspector;


namespace Studio.Game
{
    public interface IHealthBarGUI
    {
        RectTransform frame { get; }
        Graphic bar { get; }
        Graphic bossIcon { get; }

        void SetTextColor(Color color);
    }


    [CreateAssetMenu(menuName = "Crypto TF/Game/UI/Health Bar")]
    public class UIHealthBarScriptable : ScriptableObject
    {
        [SerializeField]
        private UIHealthBar _asset;
        [SerializeField]
        private Vector2 _size;
        [SerializeField]
        private bool _usePlayerColor;
        [SerializeField, ShowIfNot(nameof(_usePlayerColor))]
        private Color _barColor = new Color32(250, 176, 55, 255);
        [SerializeField, ShowIfNot(nameof(_usePlayerColor))]
        private bool _isBoss;
        [SerializeField]
        private Color _damageColor = new Color32(254, 200, 21, 255);


        public UIHealthBar Generate(Canvas canvas, Transform parent)
        {
            if (_asset)
            {
                var asset = Instantiate(_asset, parent).SetCanvas(canvas);

                SetHealthBarGUI(asset);

                return asset;
            }

            return null;
        }


        private void SetHealthBarGUI(IHealthBarGUI gui)
        {
            if (gui is not null)
            {
                if (gui.frame)
                {
                    gui.frame.sizeDelta = _size;
                }

                if (gui.bar)
                {
                    gui.bar.color = _barColor;
                }

                if (gui.bossIcon)
                {
                    gui.bossIcon.enabled = _isBoss;
                }

                gui.SetTextColor(_damageColor);
            }
        }


        private void SetHealthBarGUI(IHealthBarGUI gui, ActorPlayerScriptable actor)
        {
            if (gui is not null)
            {
                if (gui.frame)
                {
                    gui.frame.sizeDelta = _size;
                }

                if (actor)
                {
                    actor.SetRepresentColor(gui.bar);
                }

                if (gui.bossIcon)
                {
                    gui.bossIcon.enabled = _isBoss;
                }

                gui.SetTextColor(_damageColor);
            }
        }
    }
}