using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Studio.Game
{
    public class UITeamListTabView : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private Transform _enableCircle;
        [SerializeField]
        private Image[] _circleDecos;
        [SerializeField]
        private UIPlayerCard[] _cards;
        [SerializeField]
        private UIPlayerShortCard[] _shortCards;
        [SerializeField]
        private UITeamListRemovePanel _removePanel;

        private bool _isTeamEdit;
        private int _centered;
        private ActorPlayerCollection _playersAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }

            if (Global<User>.hasInstance)
            {
                Global<User>.instance.onTeamPlayerDuplicateRemoved -= OnTeamPlayerDuplicateRemoved;
            }
        }


        private void Start()
        {
            Global<User>.instance.onTeamPlayerDuplicateRemoved += OnTeamPlayerDuplicateRemoved;
            
            GenerateList();
            GenerateShortList();
            OnPanelCentered(0, 0);
            OnTeamSettingToggleChanged(_isTeamEdit = false);

            if (_removePanel && playersAsset)
            {
                _removePanel.SetEmptyScriptable(playersAsset.empty);
            }

            if (_enableCircle)
            {
                _enableCircle.DORotate(new Vector3(0f, 0f, 180f), 3f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1)
                    .SetLink(gameObject);
            }
        }


        public void OnTeamSettingToggleChanged(bool isOn)
        {
            _isTeamEdit = isOn;

            var count = _cards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_cards[i])
                {
                    _cards[i].SetEnableDraggableCard(_isTeamEdit);
                }
            }

            count = _shortCards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_shortCards[i])
                {
                    _shortCards[i].SetEnableDraggableCard(_isTeamEdit && i == _centered);
                }
            }

            count = _circleDecos?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_circleDecos[i])
                {
                    _circleDecos[i].enabled = _isTeamEdit;
                }
            }
        }


        public void OnEventLeaveTeam(UIPlayerCard card)
        {
            var count = _cards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (card == _cards[i])
                {
                    Global<User>.instance.RemoveTeamPlayer(i);
                    break;
                }
            }
        }


        public void OnPanelCentered(int centered, int _)
        {
            var count = _shortCards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_shortCards[i])
                {
                    _shortCards[i].SetEnableDraggableCard(_isTeamEdit && i == centered);
                }
            }

            _centered = centered;
        }


        private void GenerateList()
        {
            if (playersAsset && Global<User>.instance.TryGetTeamPlayers(out var teamPlayers))
            {
                var count = _cards?.Length;

                for (var i = 0; i < count; ++i)
                {
                    if (_cards[i] && playersAsset.TryGetActorPlayerScriptable(teamPlayers[i]?.actorName, out var player))
                    {
                        _cards[i].Construction(player, false);
                    }
                }
            }
        }


        private void GenerateShortList()
        {
            if (playersAsset)
            {
                var count = _shortCards?.Length;
                var players = playersAsset.GetActorNames();

                if (count == players?.Count)
                {
                    for (var i = 0; i < count; ++i)
                    {
                        if (_shortCards[i] && playersAsset.TryGetActorPlayerScriptable(players[i], out var player))
                        {
                            _shortCards[i].Construction(player, false);
                            _shortCards[i].SetEnableDroppableCard(false);
                        }
                    }
                }
            }
        }


        private void OnTeamPlayerDuplicateRemoved(int index)
        {
            if (0 <= index && index < _cards?.Length && _cards[index])
            {
                _cards[index].Construction(playersAsset.empty, false);
            }
        }
    }
}