using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;


namespace Studio.Game
{
    public interface IStoryActorRepository
    {
        void RegisterStoryActor(int index, Actor actor);
    }


    public class StoryEpisodeContext : MonoBehaviour, IStoryActorRepository
    {
        [SerializeField]
        private StoryCollectionScriptable.AssetReference _stories;
        [SerializeField]
        private AssetReferenceGameObject _gui;

        private readonly Dictionary<int, Actor> _actors = new();
        private StoryCollectionScriptable _storiesAsset;
        private StoryEpisodeScriptable _episode;
        private UIStoryEpisode _guiAsset;
        private GameObject _background;
        private Transform _camera;
        private bool _trackingCamera;

        public StoryCollectionScriptable storiesAsset => AssetScriptable.GetLoadAsset(_stories, ref _storiesAsset);



        private void OnDestroy()
        {
            if (_storiesAsset)
            {
                AssetScriptable.ReleaseAsset(_stories);
            }

            if (_guiAsset)
            {
                AssetScriptable.ReleaseAsset(_gui);
            }

            if (_background)
            {
                Destroy(_background);
            }

            foreach (var actor in _actors.Values)
            {
                if (actor)
                {
                    Destroy(actor.gameObject);
                }
            }

            _actors.Clear();
        }


        private void Start()
        {
            _camera = Camera.main.transform;

            if (storiesAsset && storiesAsset.TryGetStoryContent(Global<User>.instance.storyIndex, out var scriptable))
            {
                if (_episode = scriptable as StoryEpisodeScriptable)
                {
                    _background = _episode.GenerateBackground(_camera);
                }
            }

            _guiAsset = Instantiate(AssetScriptable.LoadAsset<GameObject>(_gui)).GetComponent<UIStoryEpisode>();

            Global<User>.instance.StoryCleared();
            StartCoroutine(OnStage());
        }


        private void Update()
        {
            if (_trackingCamera && _actors.ContainsKey(0))
            {
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, new(
                    _actors[0].transform.position.x + 2.5f,
                    _camera.transform.position.y,
                    _camera.transform.position.z), Time.deltaTime * 18f);
            }
        }


        void IStoryActorRepository.RegisterStoryActor(int index, Actor actor)
        {
            if (actor)
            {
                _actors[index] = actor;

                if (_guiAsset)
                {
                    _guiAsset.GenerateSpeech(index, actor.hud);
                }
            }
        }


        private IEnumerator OnStage()
        {
            yield return Load();

            if (_guiAsset)
            {
                yield return _guiAsset.WaitForStart();
            }

            var wfs = new WaitForSeconds(1f);

            yield return MoveBrown(wfs);
            yield return Conversation(wfs);

            if (_actors.ContainsKey(0) && _actors[0] is ActorPlayer brown && _guiAsset)
            {
                brown.Run();
                brown.SetFace(PlayerFace.Angry);
                brown.transform.DOMoveX(160f, 8f)
                    .SetLink(brown.gameObject);

                _guiAsset.OnSpeech(0, "가즈<size=58>아</size><size=62>아</size><size=66>아</size><size=70>아-!!!</size>", () =>
                {
                    _guiAsset.FadeOutScreen(() =>
                    {
                        HideBackground();
                        SceneContext.GotoContent(GameScene.HOME);
                    });
                });
            }
            else
            {
                HideBackground();
                SceneContext.GotoContent(GameScene.HOME);
            }
        }


        private IEnumerator Load()
        {
            if (_episode)
            {
                yield return _episode.GenerateActors(this);
            }
        }


        private IEnumerator MoveBrown(YieldInstruction delayComplete)
        {
            var arrived = false;

            if (_actors.ContainsKey(0) && _actors[0] is ActorPlayer brown && _guiAsset)
            {
                brown.Run();
                brown.SetFace(PlayerFace.Angry);

                _guiAsset.OnNarration("어느 날...", () =>
                {
                    if (brown)
                    {
                        brown.transform.DOMoveX(30f, 5f)
                            .SetEase(Ease.OutSine)
                            .OnComplete(() =>
                            {
                                brown.Standby();
                            })
                            .SetLink(brown.gameObject);
                    }

                    _trackingCamera = true;
                    _guiAsset.OnSpeech(0, "으아아<size=58>아</size><size=62>아</size><size=66>아</size><size=70>아-!!!</size>", () =>
                    {
                        brown.SetFace(PlayerFace.Normal);
                        _guiAsset.OnSpeech(0, "!!", () => arrived = true);
                    });
                });
            }
            else
            {
                arrived = true;
            }

            yield return new WaitUntil(() => arrived);
            yield return delayComplete;
        }


        private IEnumerator Conversation(YieldInstruction delayComplete)
        {
            if (_actors.ContainsKey(0) && _actors[0] is ActorPlayer brown &&
                _actors.ContainsKey(1) && _actors[1] is ActorPlayer cony &&
                _actors.ContainsKey(2) && _actors[2] is ActorPlayer moon &&
                _guiAsset)
            {
                var speeched = false;

                _guiAsset.OnSpeech(0, "너희들.", () =>
                {
                    cony.Flip();
                    moon.FlipHead();

                    _guiAsset.OnSpeech(1, "?");
                    _guiAsset.OnSpeech(2, "?", () => speeched = true);
                });

                yield return new WaitUntil(() => speeched);

                speeched = false;
                brown.Cheer();

                _guiAsset.OnSpeech(0, "내 동료가 되라.", () =>
                {
                    _guiAsset.OnSpeech(1, "?!");
                    _guiAsset.OnSpeech(2, "?!", () => speeched = true);
                });

                yield return new WaitUntil(() => speeched);
                yield return delayComplete;
            }
        }


        private void HideBackground()
        {
            if (_background)
            {
                var renderers = _background.GetComponentsInChildren<SpriteRenderer>();
                var count = renderers?.Length;

                for (var i = 0; i < count; ++i)
                {
                    if (renderers[i])
                    {
                        renderers[i].enabled = false;
                    }
                }
            }
        }
    }
}