using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;
using CustomInspector;


namespace Studio.Game
{
    [Serializable]
    public class UIPhaseProgress
    {
        [SerializeField]
        protected Transform _phaseRoot;
        [SerializeField, Indent(1)]
        protected Image _progress;
        [SerializeField, Indent(1)]
        protected GameObject _point, _pointLarge, _pointBoss;
        [SerializeField]
        protected TMP_Text _title;

        public TMP_Text title => _title;


        public void InitProgress(bool[] hasBosses)
        {
            if (_phaseRoot)
            {
                var count = hasBosses?.Length ?? 0;
                var width = (_phaseRoot as RectTransform).sizeDelta.x;
                var interval = width / count;

                if (_progress)
                {
                    _progress.fillAmount = 0f;
                }

                for (var i = 0; i < count; ++i)
                {
                    var point = hasBosses[i] ? _pointBoss : i == (count - 1) ? _pointLarge : _point;

                    if (point)
                    {
                        (UnityEngine.Object.Instantiate(point, _phaseRoot).transform as RectTransform).anchoredPosition = new(interval * (i + 1), 0f);
                    }
                }
            }
        }


        public void UpdateProgress(int phaseCount, int phaseCurrent, float duration)
        {
            if (_progress)
            {
                _progress.DOFillAmount((float)(phaseCurrent + 1) / phaseCount, duration)
                    .SetLink(_progress.gameObject);
            }
        }
    }
}