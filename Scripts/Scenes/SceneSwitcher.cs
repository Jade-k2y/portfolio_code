using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Studio.Game
{
    public static class SceneSwitcher
    {
        public struct Model
        {
            public GameScene scene;
            public GameScene[] keepScenes;
            public Action onPreprocess;
            public Action onPostprocess;
        }


        private static readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _scenes = new();


        public static void GotoTitle()
        {
            var key = GameScene.TITLE.ToString();
            var handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single);

            handle.Completed += loaded =>
            {
                _scenes.Clear();
                _scenes[key] = loaded;
            };
        }


        public static IEnumerator AddInstanceUI()
        {
            yield return new WaitUntil(() => AssetScriptable.isReady);

            var key = GameScene.INSTANCE_UI.ToString();

            if (!_scenes.ContainsKey(key))
            {
                Addressables.LoadSceneAsync(GameScene.INSTANCE_UI.ToString(), LoadSceneMode.Additive).Completed += scene =>
                {
                    _scenes[key] = scene;
                };
            }
        }


        public static IEnumerator LoadContents(GameScene scene)
        {
            var key = scene.ToString();
            var handle = Addressables.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);

            handle.Completed += loaded =>
            {
                _scenes.Clear();
                _scenes[key] = loaded;
            };

            yield return handle;
        }


        public static IEnumerator LoadContents(Model model)
        {
            model.onPreprocess?.Invoke();

            var key = model.scene.ToString();
            var handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Additive);

            yield return handle;
            yield return UnloadContents(model.keepScenes);

            _scenes[key] = handle;

            model.onPostprocess?.Invoke();
        }


        /*
         * https://issuetracker.unity3d.com/issues/failed-to-remove-scene-from-addressables-profiler-warning-when-using-addressables-dot-loadsceneasync-and-then-unloadasync
         */
        private static IEnumerator UnloadContents(GameScene[] keepScenes)
        {
            var selects = keepScenes?.Select(x => x.ToString())?.ToList() ?? new List<string>();
            var unloads = _scenes.Where(x => !selects.Contains(x.Key)).ToList();

            if (0 < unloads.Count)
            {
                foreach (var unload in unloads)
                {
                    var handle = Addressables.UnloadSceneAsync(unload.Value, autoReleaseHandle: true);

                    handle.Completed += _ =>
                    {
                        _scenes.Remove(unload.Key);
                    };

                    yield return handle;
                }

                yield return Resources.UnloadUnusedAssets();
            }
        }
    }
}