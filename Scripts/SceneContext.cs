using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;


namespace Studio.Game
{
    public class SceneContext : MonoBehaviour
    {
        private static SceneContext _instance;
        private static Action<GameScene> _gotoScene;

        [SerializeField]
        private GameScene _gameScene;
        [SerializeField]
        private List<GameScene> _activeScenes;
        [SerializeField]
        private StoryCollectionScriptable.AssetReference _stories;

        private StoryCollectionScriptable _storiesAsset;

        public StoryCollectionScriptable storiesAsset => AssetScriptable.GetLoadAsset(_stories, ref _storiesAsset);


        public static void GotoTitle()
        {
            UIMenuPlay.LastPlayMenu = 0;
            SceneSwitcher.GotoTitle();
        }


        public static void GotoContent(GameScene content) => _gotoScene?.Invoke(content);


        public static void GotoStory()
        {
            var index = Global<User>.hasInstance ? Global<User>.instance.storyIndex : -1;
            var asset = _instance ? _instance.storiesAsset : null;

            if (asset && asset.TryGetStoryContent(index, out var content))
            {
                GotoContent(content.gameScene);
            }
            else
            {
                GotoContent(GameScene.STORY_STAGE);
            }
        }


        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;


        private void OnDestroy()
        {
            if (_instance)
            {
                if (_instance._storiesAsset)
                {
                    AssetScriptable.ReleaseAsset(_stories);
                }

                _instance = null;
            }
        }


        private void Awake() => _instance = this;

        
        private void Start()
        {
            _gotoScene = target =>
            {
                var switchModel = new SceneSwitcher.Model()
                {
                    scene = target,
                    keepScenes = new[] { GameScene.GAME, GameScene.INSTANCE_UI }
                };
                StartCoroutine(SceneSwitcher.LoadContents(switchModel));
            };

            GotoContent(GameScene.HOME);
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            var target = (GameScene)Enum.Parse(typeof(GameScene), scene.name);

            if (_activeScenes?.Contains(target) ?? false)
            {
                SceneManager.SetActiveScene(scene);
            }
        }
    }
}