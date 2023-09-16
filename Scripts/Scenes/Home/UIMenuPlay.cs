using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using UniRx;


namespace Studio.Game
{
    public class UIMenuPlay : UIMenu
    {
        [SerializeField]
        private SimpleScrollSnap _scrollSnap;
        [SerializeField]
        private CanvasGroup[] _menus;
        [SerializeField]
        private Button _packageSale, _packageGold;

        public static int LastPlayMenu = 0;

        private int _visitCount = 0;
        private bool _popupable = false;
        private readonly Queue<ITutorial> _tutorials = new();


        public override Menu menu => Menu.Play;


        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;


        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            var tutorials = GetComponents<ITutorial>()
                ?.Where(x => !x.isDone && !x.useTrigger)
                ?.OrderBy(x => x.order);

            foreach (var tutorial in tutorials)
            {
                _tutorials.Enqueue(tutorial);
            }
        }


        protected override void Start()
        {
            base.Start();

            _packageSale.SetClickEvent(() => GotoMenu(Menu.Shop));
            _packageGold.SetClickEvent(() => GotoMenu(Menu.Shop));

            if (_scrollSnap)
            {
                _scrollSnap.GoToPanel(LastPlayMenu);
            }

            OnPlayMenuCentered(LastPlayMenu, LastPlayMenu);
        }


        public override void OnMenuFocused(UIMenu prevMenu)
        {
            if (0 >= _visitCount++ && 0 < _tutorials.Count)
            {
                StartCoroutine(ConfirmTutorial());
            }
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
            var target = (GameScene)Enum.Parse(typeof(GameScene), scene.name);

            if (target is GameScene.INSTANCE_UI)
            {
                _popupable = true;
            }
        }


        private IEnumerator ConfirmTutorial()
        {
            yield return new WaitUntil(() => _popupable);

            Popup<UIPopupConfirm>.instance.OnPopup(new UIPopupConfirm.Model(
                "튜토리얼 확인",
                "튜토리얼이 필요 하십니까?",
                () => StartCoroutine(RunTutorials()),
                () => Global<User>.instance.isShowTutorial = false));
        }

        private IEnumerator RunTutorials()
        {
            yield return new WaitUntil(() => _popupable);

            while (0 < _tutorials.Count)
            {
                yield return _tutorials.Dequeue().Play();
            }
        }


        public void OnPlayMenuCentered(int centered, int _)
        {
            LastPlayMenu = centered;

            var count = _menus?.Length;

            for (var i = 0; i < count; ++i)
            {
                _menus[i].SetInteractable(i == centered);
            }
        }
    }
}