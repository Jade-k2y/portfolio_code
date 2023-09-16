using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Studio.Game
{
    [RequireComponent(typeof(GraphicRaycaster), typeof(CanvasGroup))]
    public abstract class UIMenu : MonoBehaviour
    {
        public enum Menu
        {
            Shop, Character, Play, Ranking, Community
        }


        [SerializeField]
        protected UISceneHome _homeScene;
        [SerializeField]
        protected Image _background;


        public abstract Menu menu { get; }


        protected virtual void Start()
        {
            if (_background)
            {
                _background.DOColor(new Color(1f, _background.color.g, _background.color.b, _background.color.a), 12f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutBounce)
                    .SetLink(gameObject);
            }
        }


        public abstract void OnMenuFocused(UIMenu prevMenu);


        public virtual void GotoMenu(Menu targetMenu)
        {
            if (_homeScene)
            {
                _homeScene.OnMoveMenu(targetMenu);
            }
        }
    }
}