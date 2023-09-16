using UnityEngine;
using UnityEngine.UI;
using System;


namespace Studio.Game
{
    [Popup(nameof(UIPopupCombination), "Popups", destroyOnDisabled: true)]
    public class UIPopupCombination : UIPopup
    {
        public class Model : IPopupModel
        {
            public Action onClose { get; private set; }

            public Model(Action onClose)
            {
                this.onClose = onClose;
            }
        }

        [Serializable]
        public class Layer
        {
            public Image frame;
            public Image thumbnail;
            public Vector2 relative;
        }

        [SerializeField]
        private ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        private Layer[] _layers;
        [SerializeField]
        private float _duration;
        [SerializeField]
        private GameObject _fx;

        private ActorPlayerCollection _playersAsset;

        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);


        private void OnDestroy()
        {
            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }
        }


        private void Awake()
        {
            if (0f > _duration)
            {
                _duration = 2f;
            }
        }


        protected override void Start()
        {
            base.Start();

            if (playersAsset && GameConstant.MaxPlayerSlotCount <= _layers?.Length && Global<User>.instance.TryGetTeamPlayers(out var team))
            {
                for (var i = 0; i < GameConstant.MaxPlayerSlotCount; ++i)
                {
                    if (team[i].isExist && playersAsset.TryGetActorPlayerScriptable(team[i].actorName, out var actor))
                    {
                        /*
                        actor.SetRepresentColor(_layers[i].frame);
                        */
                        actor.SetThumbnail(_layers[i]?.thumbnail, true, (PlayerThumbnail)UnityEngine.Random.Range(0, (int)PlayerThumbnail.COUNT));
                    }
                    else
                    {
                        if (_layers[i].frame)
                        {
                            _layers[i].frame.gameObject.SetActive(false);
                        }
                    }
                }
            }

            Invoke(nameof(Close), _duration);
        }


        private void Close()
        {
            if (_model is Model model)
            {
                model.onClose?.Invoke();
            }

            OnClose();
        }
    }
}