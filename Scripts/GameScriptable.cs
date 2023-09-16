using UnityEngine;
using System;
using DG.Tweening;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Common/Game")]
    public class GameScriptable : ScriptableObject
    {
        public interface ISetting : IDisposable
        {
            void Initialize();
        }

        #region quality setting
        [Serializable]
        public class QualitySetting : ISetting
        {
            public int targetFrameRate = 60;
            public int mobileVSyncCount = 0;

            public void Initialize()
            {
#if UNITY_IOS || UNITY_ANDROID
                QualitySettings.vSyncCount = mobileVSyncCount;
                Application.targetFrameRate = targetFrameRate;
#endif
            }

            void IDisposable.Dispose() { }
        }
        #endregion //quality setting

        #region dotween setting
        [Serializable]
        public class DOTweenSetting : ISetting
        {
            public bool recycleAllByDefault;
            public bool useSafeMode;
            public LogBehaviour logBehaviour = LogBehaviour.Default;
            public int tweenersCapacity = 200;
            public int sequencesCapacity = 50;

            public void Initialize() => DOTween.Init(recycleAllByDefault, useSafeMode, logBehaviour)
                .SetCapacity(tweenersCapacity, sequencesCapacity);

            public void Dispose() => DOTween.Clear(true);
        }
        #endregion //dotween setting

        #region runtime setting
        [Serializable]
        public class RuntimeSetting : ISetting
        {
            [SerializeField]
            private float _1xSpeed, _2xSpeed;

            public void Initialize() => Time.timeScale = _1xSpeed;

            public void Dispose() { }

            public void SetTimeSclae(bool x2) => Time.timeScale = x2 ? _2xSpeed : _1xSpeed;
        }
        #endregion //runtime setting


        public static GameScriptable _instance;

        [SerializeField]
        private QualitySetting _quality;
        [SerializeField]
        private DOTweenSetting _dotween;
        [SerializeField]
        private RuntimeSetting _runtime;
        [SerializeField]
        private bool _isTutorialUnitTest;

        public static bool isTutorialUnitTest
#if UNITY_EDITOR
            => _instance && _instance._isTutorialUnitTest;
#else
            => false;
#endif

        private void OnDisable()
        {
            DOTween.Clear(true);

            _instance = null;
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        public static void LoadInstance() => _instance = Resources.Load<GameScriptable>(nameof(GameScriptable));
#else
        private void OnEnable() => _instance = this;
#endif


        public static void Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (_instance)
            {
                _instance._quality?.Initialize();
                _instance._dotween?.Initialize();
                _instance._runtime?.Initialize();
            }

            Debug.Log("GameScriptable initialized.");
        }


        public static void SetTimeSclae(bool x2)
        {
            if (_instance)
            {
                _instance._runtime?.SetTimeSclae(x2);
            }
        }
    }
}