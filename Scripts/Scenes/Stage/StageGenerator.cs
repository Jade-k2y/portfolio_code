using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Studio.Game
{
    public class StageGenerator : MonoBehaviour
    {
        public struct StageBridge
        {
            public int index;
            public int level;
            public string title;

            public StageBridge(int index, int level, string title)
            {
                this.index = index;
                this.level = level;
                this.title = title;
            }
        }

        [SerializeField]
        private BackgroundCollectionScriptable.AssetReference _background;
        [SerializeField]
        private EnvironmentCollectionScriptable.AssetReference _environment;
        [SerializeField]
        private FormationCollectionScriptable.AssetReference _formation;
        [SerializeField]
        private ActorMonsterCollection.AssetReference _monsters;

        private BackgroundCollectionScriptable _backgroundAsset;
        private EnvironmentCollectionScriptable _environmentAsset;
        private FormationCollectionScriptable _formationAsset;
        private ActorMonsterCollection _monstersAsset;

        public BackgroundCollectionScriptable backgroundAsset => AssetScriptable.GetLoadAsset(_background, ref _backgroundAsset);
        public EnvironmentCollectionScriptable environmentAsset => AssetScriptable.GetLoadAsset(_environment, ref _environmentAsset);
        public FormationCollectionScriptable formationAsset => AssetScriptable.GetLoadAsset(_formation, ref _formationAsset);
        public ActorMonsterCollection monstersAsset => AssetScriptable.GetLoadAsset(_monsters, ref _monstersAsset);

        #region static
        private static StageGenerator _instance;

        public static StageBridge bridge { get; private set; }

        public static T GenerateStage<T>(GameScene scene, string title) where T : StageScriptable
            => ScriptableObject.CreateInstance<T>()
            .GetExtra(scene, title, _instance) as T;

        public static void SetStageBridge(int index, int level, string title) => bridge = new StageBridge(index, level, title);
        #endregion


        private void OnDestroy()
        {
            if (_instance)
            {
                if (_backgroundAsset)
                {
                    AssetScriptable.ReleaseAsset(_background);
                }

                if (_environmentAsset)
                {
                    AssetScriptable.ReleaseAsset(_environment);
                }

                if (_formationAsset)
                {
                    AssetScriptable.ReleaseAsset(_formation);
                }

                if (_monstersAsset)
                {
                    AssetScriptable.ReleaseAsset(_monsters);
                }

                _instance = null;
            }
        }


        private void Awake() => _instance = this;


        public BackgroundScriptable.AssetReference GetRandomBackgroundReference()
            => backgroundAsset ? backgroundAsset.GetRandomElementAssetReference() : default;

        public EnvironmentScriptable.AssetReference GetRandomEnvironmentReference()
            => environmentAsset ? environmentAsset.GetRandomElementAssetReference() : default;

        public FormationScriptable.AssetReference GetPlayerFormationReference(GameScene scene)
            => formationAsset ? formationAsset.GetPlayerFormationReference(scene) : default;

        public FormationScriptable.AssetReference GetRandomFormationReference()
            => formationAsset ? formationAsset.GetRandomElementAssetReference() : default;

        public ActorScriptable.AssetReference GetRandomMonster()
            => monstersAsset ? monstersAsset.GetRandomActorAssetReference() : null;
    }
}