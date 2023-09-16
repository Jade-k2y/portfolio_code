using UnityEngine;
using System;
using CustomInspector;
using UnityEngine.SceneManagement;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Camera")]
    public class CameraScriptable : ScriptableObject
    {
        #region init
        [Serializable]
        public class Init
        {
            [SerializeField, ReadOnly]
            private string _name;
            [SerializeField, Validate(nameof(IsValidate))]
            private GameScene _scene;
            [SerializeField]
            private Vector3 _position;
            [SerializeField]
            private Vector3 _rotation;
            [SerializeField]
            private Vector3 _scale;
            [SerializeField]
            private bool _orthographic;
            [SerializeField, ShowIf(nameof(_orthographic))]
            private float _size;
            [SerializeField, ShowIfNot(nameof(_orthographic))]
            private Camera.FieldOfViewAxis _fieldOfViewAxis;
            [SerializeField, Range(0f, 180f), ShowIfNot(nameof(_orthographic))]
            private float _fieldOfView;
            [SerializeField]
            private float _clippingPlanesNear, _clippingPlanesFar;

            private bool IsValidate(GameScene scene) => scene switch
            {
                GameScene.GAME => false,
                GameScene.INSTANCE_UI => false,
                GameScene.TITLE => false,
                _ => true
            };

            public void Default()
            {
                _name = _scene.ToString();
            }

            public bool Is(Scene scene) => _scene.ToString().Equals(scene.name);

            public void SetCamera(Camera target)
            {
                if (target)
                {
                    target.transform.SetPositionAndRotation(_position, Quaternion.Euler(_rotation));
                    target.transform.localScale = _scale;

                    if (target.orthographic = _orthographic)
                    {
                        target.orthographicSize = _size;
                    }
                    else
                    {
                        if (_fieldOfViewAxis is Camera.FieldOfViewAxis.Horizontal)
                        {
                            target.fieldOfView = Camera.HorizontalToVerticalFieldOfView(_fieldOfView, target.aspect);
                        }
                        else if (_fieldOfViewAxis is Camera.FieldOfViewAxis.Vertical)
                        {
                            target.fieldOfView = Camera.VerticalToHorizontalFieldOfView(_fieldOfView, target.aspect);
                        }
                    }

                    target.nearClipPlane = _clippingPlanesNear;
                    target.farClipPlane = _clippingPlanesFar;
                }
            }
        }
        #endregion


        [SerializeField]
        private Init[] _inits;


        private void OnValidate()
        {
            if (_inits is not null)
            {
                foreach (var init in _inits)
                {
                    init?.Default();
                }
            }
        }


        public void OnActiveSceneChanged(Scene _, Scene next)
        {
            var camera = Camera.main;

            if (camera)
            {
                var count = _inits?.Length;

                for (var i = 0; i < count; ++i)
                {
                    if (_inits[i]?.Is(next) ?? false)
                    {
                        _inits[i].SetCamera(camera);
                        break;
                    }
                }
            }
        }
    }
}