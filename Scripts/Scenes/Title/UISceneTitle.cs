using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UI.MTP;


namespace Studio.Game
{
    public class UISceneTitle : UIScene
    {
        [SerializeField]
        private CanvasGroup _groupTitle, _groupStart;
        [SerializeField]
        private StyleManager _styleTitle;
        [SerializeField]
        private float _delayTitle = 1f;
        [SerializeField]
        private float _delayStart = 2f;
        [SerializeField]
        private Button _tapToStart;

        private Coroutine _coroutine = null;


        private void Awake()
        {
            Global<User>.instance.Initialize(true);
        }


        protected override void Start()
        {
            base.Start();

            _groupTitle.Hide();
            _groupStart.Hide();

            StartCoroutine(DelayAction(_delayTitle, () =>
            {
                _groupTitle.Show();

                if (_styleTitle)
                {
                    _styleTitle.Play();
                }
            }));
            StartCoroutine(DelayAction(_delayStart, () => StartCoroutine(CheckUpdate())));

            _coroutine = default;
            _tapToStart.SetClickEvent(OnTabToStart, false);
        }


        private IEnumerator DelayAction(float delay, Action onCallback)
        {
            if (0f < delay)
            {
                var wfs = new WaitForSeconds(delay);

                yield return wfs;
            }

            onCallback?.Invoke();
        }


        private IEnumerator CheckUpdate()
        {
            yield return AssetScriptable.CheckForAddressableUpdate();
            yield return AssetScriptable.CheckForDownladSize(size =>
            {
                if (0 < size)
                {
                    Popup<UIPopupDownload>.instance.OnPopup(new UIPopupDownload.Model(size, () => _groupStart.Show()));
                }
                else
                {
                    _groupStart.Show();
                }
            });
        }


        private void OnTabToStart()
        {
            if (_groupStart.IsVisible())
            {
                _coroutine ??= StartCoroutine(SceneSwitcher.LoadContents(GameScene.GAME));
            }
        }
    }
}