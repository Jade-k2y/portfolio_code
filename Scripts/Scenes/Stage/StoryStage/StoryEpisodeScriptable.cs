using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using System.Collections;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Story/Episode")]
    public class StoryEpisodeScriptable : StageScriptable
    {
        [Serializable]
        public class StoryActor
        {
            [HideInInspector]
            public string name;
            public ActorScriptable.AssetReference _actor;
            public Vector3 position;
            public Vector3 scale;

            private ActorScriptable _actorAsset;

            public ActorScriptable actorAsset => AssetScriptable.GetLoadAsset(_actor, ref _actorAsset);

            public void OnDestroy()
            {
                if (_actorAsset)
                {
                    AssetScriptable.ReleaseAsset(_actor);
                }
            }
        }

        [SerializeField]
        private AssetReferenceGameObject _background;
        [SerializeField]
        private ParallaxBackgroundScriptable.AssetReference _parallax;
        [SerializeField]
        private StoryActor[] _storyActors;

        private GameObject _backgroundAsset;
        private ParallaxBackgroundScriptable _parallaxAsset;

        public GameObject backgroundAsset => AssetScriptable.GetLoadAsset(_background, ref _backgroundAsset);
        public ParallaxBackgroundScriptable parallaxAsset => AssetScriptable.GetLoadAsset(_parallax, ref _parallaxAsset);


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_backgroundAsset)
            {
                AssetScriptable.ReleaseAsset(_background);
            }

            var count = _storyActors?.Length;

            for (var i = 0; i < count; ++i)
            {
                _storyActors[i]?.OnDestroy();
            }
        }


        protected override bool IsValidate(GameScene scene) => scene is GameScene.STORY_EPISODE;


        public GameObject GenerateBackground(Transform parent)
        {
            if (backgroundAsset)
            {
                var background = Instantiate(backgroundAsset, parent);

                if (parallaxAsset)
                {
                    parallaxAsset.SetParallaxBackground(background.GetComponentsInChildren<IParallaxRenderer>());
                }

                return background;
            }

            return null;
        }


        public IEnumerator GenerateActors(IStoryActorRepository repository)
        {
            var wfef = new WaitForEndOfFrame();
            var index = 0;

            foreach (var storyActor in _storyActors)
            {
                if (storyActor?.actorAsset && storyActor.actorAsset.TryGenerateActor(out var actor))
                {
                    actor.transform.position = storyActor.position;
                    actor.transform.localScale = storyActor.scale;

                    repository.RegisterStoryActor(index++, actor.GetComponent<Actor>());

                    yield return wfef;
                }
            }
        }
    }
}