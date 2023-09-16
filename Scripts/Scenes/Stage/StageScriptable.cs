using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.AddressableAssets;
using System;
using TMPro;
using CustomInspector;


namespace Studio.Game
{
    public abstract class StageScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<StageScriptable>
        {
            public AssetReference(string guild) : base(guild) { }
        }

        [SerializeField, Validate(nameof(IsValidate))]
        protected GameScene _gameScene;
        [SerializeField]
        protected string _title;
        [SerializeField]
        protected EnvironmentScriptable.AssetReference _environment;

        private EnvironmentScriptable _environmentAsset;

        public GameScene gameScene => _gameScene;
        public EnvironmentScriptable environmentAsset => AssetScriptable.GetLoadAsset(_environment, ref _environmentAsset);


        protected virtual void OnDestroy()
        {
            if (_environmentAsset)
            {
                AssetScriptable.ReleaseAsset(_environment);
            }
        }


        protected abstract bool IsValidate(GameScene scene);


        public virtual StageScriptable GetExtra(GameScene gameScene, string title, StageGenerator generator)
        {
            _gameScene = gameScene;
            _title = title;

            if (generator)
            {
                _environment = generator.GetRandomEnvironmentReference();
            }

            return this;
        }


        public void SetTitle(TMP_Text target)
        {
            if (target)
            {
                target.text = _title;
            }
        }


        public void SetEnvironment(Light2D mainLight)
        {
            if (environmentAsset)
            {
                environmentAsset.SetLight(mainLight);
            }
        }
    }
}