using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


namespace Studio.Game
{
    public class UIGearCard : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Graphic _innerFrame;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private GameObject _fx;
        [SerializeField]
        private TMP_Text _gearName;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TMP_Text _levelTxt;
        [SerializeField]
        private MasteryScriptable.AssetReference _mastery;
        [SerializeField]
        private TMP_Text _masteryName;
        [SerializeField]
        private Image _masteryIcon;
        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private Image _actorThumbnail;
        [SerializeField]
        private TMP_Text _hp, _atk, _def, _int, _res, _spd, _crt, _crd;

        private GearScriptable.Gear _model;
        private int _index;
        private int _level;
        private bool _gotoDungeon;
        private ActorPlayerCollection _playersAsset;
        private MasteryScriptable _masteryAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);
        public MasteryScriptable masteryAsset => AssetScriptable.GetLoadAsset(_mastery, ref _masteryAsset);


        private void OnValidate() => SetActiveFX(false);


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }

            if (_masteryAsset)
            {
                AssetScriptable.ReleaseAsset(_mastery);
            }
        }


        private void Start() => _button.SetClickEvent(OnClickCard, false);


        public void Construction(GearScriptable.Gear gear, int index, int level, bool gotoDungeon)
        {
            _index = index;
            _level = level;

            if ((_model = gear) is not null)
            {
                SetText(_gearName, _model.name);

                if (_icon)
                {
                    _icon.sprite = _model.icon;
                    _icon.enabled = _icon.sprite;
                }

                SetText(_levelTxt, _level.ToLevel());

                if (masteryAsset)
                {
                    masteryAsset.SetMasteryName(_masteryName, _model.gearMastery);
                    masteryAsset.SetMasteryIcon(_masteryIcon, _model.gearMastery);
                }

                if (playersAsset && playersAsset.TryGetActorPlayerScriptable(_model.ownerActorName, out var scriptable))
                {
                    scriptable.SetThumbnail(_actorThumbnail);
                    scriptable.SetRepresentColor(_innerFrame);
                }
                else
                {
                    if (_actorThumbnail)
                    {
                        _actorThumbnail.enabled = false;
                    }

                    if (_innerFrame)
                    {
                        _innerFrame.color = Color.white;
                    }
                }

                SetText(_hp, GetString(_model.stats.health));
                SetText(_atk, GetString(_model.stats.attack));
                SetText(_def, GetString(_model.stats.defense));
                SetText(_int, GetString(_model.stats.intelligence));
                SetText(_res, GetString(_model.stats.resistance));
                SetText(_spd, GetString(_model.stats.speed));
                SetText(_crt, GetString(_model.stats.criticalRate) + "%");
                SetText(_crd, GetString(_model.stats.criticalDamage) + "%");
            }

            _gotoDungeon = gotoDungeon;
        }


        public void Fading(bool fadeIn, bool instantly = false, TweenCallback onComplete = null)
        {
            if (_canvasGroup)
            {
                if (!instantly)
                {
                    _canvasGroup.DOFade(fadeIn ? 1f : 0f, GameConstant.DefaultPresentDuration * 2f)
                        .OnComplete(onComplete)
                        .SetLink(gameObject);
                }
                else
                {
                    if (fadeIn)
                    {
                        _canvasGroup.Show();
                    }
                    else
                    {
                        _canvasGroup.Hide();
                    }

                    onComplete?.Invoke();
                }
            }
        }


        public void SetActiveFX(bool active)
        {
            if (_fx)
            {
                _fx.SetActive(active);
            }
        }


        private void OnClickCard()
        {
            if (_model is not null && _gotoDungeon)
            {
                Popup<UIPopupConfirm>.instance.OnPopup(
                    new UIPopupConfirm.Model(
                        $"{_level.ToLevel()} {_model.name}",
                        "장비 세계로 입장 하시겠습니까?",
                        () =>
                        {
                            StageGenerator.SetStageBridge(_index, _level, _model.name);
                            SceneContext.GotoContent(GameScene.GEAR_STAGE);
                        }));
            }
        }


        private string GetString(int source)
        {
            if (0 < source)
            {
                return $"+{source}";
            }

            return $"{source}";
        }


        private string GetString(float source)
        {
            if (0f < source)
            {
                return $"+{source:0.##}";
            }

            return $"{source:0.##}";
        }


        private void SetText(TMP_Text target, string text)
        {
            if (target)
            {
                target.SetText(text);
            }
        }
    }
}