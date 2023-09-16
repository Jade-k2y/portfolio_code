using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections.Generic;
using System.Reflection;


namespace Studio.Game
{
    public class InstanceUI : MonoBehaviour
    {
        public static Func<GameObject, GameObject> CallbackAddChildren { get; private set; }

        [SerializeField]
        private Canvas _canvas;


        private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;


        private void Awake() => SceneManager.sceneUnloaded += OnSceneUnloaded;


        private void Start()
        {
            if (_canvas)
            {
                var mainCamera = Camera.main;
                var cameras = mainCamera ? mainCamera.GetUniversalAdditionalCameraData().cameraStack : new List<Camera>();

                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = 0 < cameras.Count ? cameras[0] : mainCamera;
                _canvas.sortingLayerName = "UI";
                _canvas.sortingOrder = 50;
            }

            CallbackAddChildren = asset => asset ? Instantiate(asset, transform) : asset;
        }


        private void OnSceneUnloaded(Scene scene)
        {
            if (_canvas)
            {
                var popups = _canvas.GetComponentsInChildren<UIPopup>(true);
                var destroyed = false;

                foreach (var popup in popups)
                {
                    if (popup)
                    {
                        var attribute = popup.GetType().GetCustomAttribute<PopupAttribute>();

                        if (!attribute.keepAlive)
                        {
                            DestroyImmediate(popup.gameObject);
                            destroyed = true;
                        }
                    }
                }

                if (destroyed)
                {
                    Resources.UnloadUnusedAssets();
                }
            }
        }
    }
}