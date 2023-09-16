using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using DG.Tweening;
using UniRx;


namespace Studio.Game
{
    public class UIUserProfile : MonoBehaviour
    {
        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private Image _background, _thumbnail, _guage;
        [SerializeField]
        private RectTransform _detailBar, _board;
        [SerializeField]
        private TMP_Text _nickname, _levelCircle, _levelBar, _exp;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private bool _hideExpand;
        [SerializeField]
        private Graphic[] _circleLevels;

        private readonly Vector3 _offBar = new(0f, 0f, 1f);
        private readonly Vector3 _offBoard = new(1f, 0f, 1f);
        private readonly float _expandHeight = 1248f;
        private Vector2 _simpleSize;
        private bool _started;
        private ActorPlayerCollection _playersAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);


        private void OnValidate()
        {
            if (_hideExpand)
            {
                if (_detailBar)
                {
                    _simpleSize = _detailBar.sizeDelta;
                    _detailBar.localScale = _offBar;
                }

                if (_board)
                {
                    _board.localScale = _offBoard;
                }
            }
        }


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }
        }


        private void Awake()
        {
            if (_detailBar)
            {
                _simpleSize = _detailBar.sizeDelta;
                _detailBar.localScale = _offBar;
            }

            if (_board)
            {
                _board.localScale = _offBoard;
            }

            if (0f >= _duration)
            {
                _duration = GameConstant.DefaultPresentDuration;
            }
        }


        private void Start()
        {
            var user = Global<User>.instance;

            user.mainPlayer.Subscribe(actorName =>
            {
                if (playersAsset && playersAsset.TryGetActorPlayerScriptable(actorName, out var scriptable))
                {
                    scriptable.SetRepresentColor(_background);
                    scriptable.SetThumbnail(_thumbnail, true);
                }
            }).AddTo(this);
            user.id.Subscribe(nickName =>
            {
                if (_nickname)
                {
                    _nickname.SetText(nickName);
                }
            }).AddTo(this);

            SetUserLevel(user.level.Value);
            Global<Table>.instance.SetUserExpGUI(_exp, _guage);

            _started = true;
        }


        private void SetUserLevel(int level)
        {
            level.ToLevel(_levelBar);
            level.ToLevel(_levelCircle);
        }


        public void OnToggleCircleChanged(bool isOn)
        {
            if (!_hideExpand && _detailBar)
            {
                _detailBar.DOScale(isOn ? Vector3.one : _offBar, _duration)
                    .OnComplete(() =>
                    {
                        if (isOn)
                        {
                            MessageBroker.Default.Publish(TutorialType.ProfileBar);
                        }
                    })
                    .SetLink(gameObject);

                var count = _circleLevels?.Length;

                for (var i = 0; i < count; ++i)
                {
                    if (_circleLevels[i])
                    {
                        _circleLevels[i].DOFade(isOn ? 0f : 1f, _duration)
                            .SetLink(_circleLevels[i].gameObject);
                    }
                }
            }
        }


        public void OnToggleBoardChanged(bool isOn)
        {
            if (!_hideExpand)
            {
                if (_detailBar)
                {
                    _detailBar.DOSizeDelta(isOn ? new Vector2(_simpleSize.x, _expandHeight) : _simpleSize, _duration)
                        .SetLink(gameObject);
                }

                if (_board)
                {
                    _board.DOScale(isOn ? Vector3.one : _offBoard, _duration)
                        .SetLink(gameObject);
                }
            }
        }


        public void OnPresentAdditionalExp(long additionalExp, Action onFinished)
        {
            StartCoroutine(PresentAdditionalExp(new WaitForSeconds(1f), additionalExp, onFinished));
        }


        private IEnumerator PresentAdditionalExp(YieldInstruction instruction, long additionalExp, Action onFinished)
        {
            yield return new WaitUntil(() => _started);
            yield return instruction;

            var user = Global<User>.instance;
            var level = user.level.Value;
            var exp = user.exp.Value;

            user.SetAdditionalExp(additionalExp);

            if (0 < level)
            {
                while (0 < additionalExp)
                {
                    var target = Global<Table>.instance.GetUserLevelRequiredExp(level);
                    var total = exp + additionalExp;

                    if (target <= total)
                    {
                        level++;
                        exp = 0;
                        additionalExp = total - target;

                        if (_guage)
                        {
                            var finished = false;

                            _guage.DOFillAmount(1f, GameConstant.DefaultPresentDuration)
                                .OnComplete(() => finished = true)
                                .SetLink(gameObject);

                            yield return new WaitUntil(() => finished);
                            SetUserLevel(level);

                            _guage.fillAmount = 0f;
                        }
                    }
                    else
                    {
                        exp = additionalExp;
                        additionalExp = 0;

                        if (_guage)
                        {
                            var required = Global<Table>.instance.GetUserLevelRequiredExp(level);
                            var percentage = 0 < exp ? (float)exp / required : 0f;
                            var finished = false;

                            _guage.DOFillAmount(percentage, GameConstant.DefaultPresentDuration)
                                .OnComplete(() => finished = true)
                                .SetLink(gameObject);

                            yield return new WaitUntil(() => finished);
                            SetUserLevel(level);
                        }
                    }
                }
            }

            onFinished?.Invoke();
        }
    }
}