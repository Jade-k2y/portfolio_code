using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;


namespace Studio.Game
{
    public abstract class UIScene : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;


        protected virtual void Start()
        {
            if (_canvas)
            {
                var mainCamera = Camera.main;
                var cameras = mainCamera ? mainCamera.GetUniversalAdditionalCameraData().cameraStack : new List<Camera>();

                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = 0 < cameras.Count ? cameras[0] : mainCamera;
            }

            StartCoroutine(SceneSwitcher.AddInstanceUI());
        }
    }
}