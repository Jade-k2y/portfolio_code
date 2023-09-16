using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    [Popup(nameof(UIPopupDownload), "Popups")]
    public class UIPopupDownload : UIPopup
    {
        #region model
        public class Model : IPopupModel
        {
            public long size { get; private set; }
            public Action onComplete { get; private set; }

            public Model(long size, Action onComplete)
            {
                this.size = size;
                this.onComplete = onComplete;
            }
        }
        #endregion

        [SerializeField]
        private TMP_Text _download;
        [SerializeField]
        private Image _progress;
        [SerializeField]
        private Button _btnDownload;


        protected override void Start()
        {
            base.Start();

            _btnDownload.SetClickEvent(OnClickDownload);
        }


        protected override void ApplyModel()
        {
            base.ApplyModel();

            if (_model is Model model)
            {
                var mbyte = (model.size / 1024f) / 1024f;

                if (_download)
                {
                    _download.SetText(0.01f > mbyte ? "0.01 MB" : $"{mbyte:0.00} MB");
                }
            }

            if (_progress)
            {
                _progress.fillAmount = 0f;
            }
        }


        private void OnClickDownload()
        {
            if (_btnDownload)
            {
                _btnDownload.interactable = false;
            }

            StartCoroutine(AssetScriptable.DownloadAssets(OnUpdateProgress, OnDownloadComplte));
        }


        private void OnUpdateProgress(float progress)
        {
            if (_download)
            {
                _download.SetText($"{progress * 100f:0.00}%");
            }

            if (_progress)
            {
                _progress.fillAmount = progress;
            }
        }


        private void OnDownloadComplte()
        {
            OnClose();

            if (_model is Model model)
            {
                model.onComplete?.Invoke();
            }
        }
    }
}