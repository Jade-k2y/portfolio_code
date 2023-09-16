using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;


namespace Studio.Game
{
    public interface IActorPresenterRepository
    {
        void RegisterActorPresenter(StageActorPresenter presenter);
    }


    public abstract class StageContext<TScriptable, TGUI> : MonoBehaviour, IActorPresenterRepository
        where TScriptable : StageScriptable
        where TGUI : UIScene
    {
        [SerializeField]
        protected StageScriptable.AssetReference _scriptable;
        [SerializeField]
        protected AssetReferenceGameObject _gui;
        [SerializeField]
        protected ActorPlayerCollection.AssetReference _players;
        [SerializeField]
        protected SpriteRenderer _background;
        [SerializeField]
        protected float _finishDelay;

        protected readonly Dictionary<int, StageActorPresenter> _presenters = new();
        protected bool _extra;
        protected bool _gameResult;
        protected int _tryCount;
        protected TScriptable _scriptableAsset;
        protected TGUI _guiAsset;
        private ActorPlayerCollection _playersAsset;

        public TScriptable scriptableAsset => _extra ? _scriptableAsset : AssetScriptable.GetLoadAsset(_scriptable, ref _scriptableAsset);
        public TGUI guiAsset => _guiAsset;
        public ActorPlayerCollection playersAsset => AssetScriptable.GetLoadAsset(_players, ref _playersAsset);


        protected virtual void OnDestroy()
        {
            if (_scriptableAsset)
            {
                if (_extra)
                {
                    Destroy(_scriptableAsset);
                }
                else
                {
                    AssetScriptable.ReleaseAsset(_scriptable);
                }
            }

            if (_guiAsset)
            {
                AssetScriptable.ReleaseAsset(_gui);
            }

            if (_playersAsset)
            {
                AssetScriptable.ReleaseAsset(_players);
            }

            _presenters?.Clear();
        }


        protected virtual void Awake() { }


        protected virtual void Start()
        {
            var asset = scriptableAsset;

            if (asset)
            {
                asset.SetEnvironment(Global<Environment>.instance.mainLight);

                if (asset is StoryStageScriptable story)
                {
                    story.SetGround(_background);
                    story.InitView();
                }
                else if (asset is PvPStageScriptable pvp)
                {
                    pvp.SetGround(_background);
                    pvp.InitView();
                }
            }

            _guiAsset = Instantiate(AssetScriptable.LoadAsset<GameObject>(_gui)).GetComponent<TGUI>();

            StartCoroutine(OnStage());
        }


        void IActorPresenterRepository.RegisterActorPresenter(StageActorPresenter presenter)
        {
            if (presenter is not null)
            {
                _presenters[presenter.formation] = presenter;
            }
        }

        
        protected IEnumerator OnStage()
        {
            if (0 == _tryCount)
            {
                yield return Load();
                yield return Intro();
            }
            else
            {
                yield return Retry();
            }
            
            yield return Play();
            yield return Finish();
        }


        protected abstract IEnumerator Load();

        protected virtual IEnumerator Intro()
        {
            var intro = false;
            var wu = new WaitUntil(() => intro);

            Popup<UIPopupCombination>.instance.OnPopup(new UIPopupCombination.Model(() => intro = true));

            yield return wu;
        }

        protected abstract IEnumerator Retry();

        protected abstract IEnumerator Play();

        protected abstract IEnumerator Finish();


        protected void GotoHome() => SceneContext.GotoContent(GameScene.HOME);
    }
}