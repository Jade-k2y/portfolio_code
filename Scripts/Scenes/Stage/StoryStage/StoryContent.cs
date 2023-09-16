using UnityEngine;
using CustomInspector;


namespace Studio.Game
{
    public abstract class StoryContent : ScriptableObject
    {
        [SerializeField, Validate(nameof(IsValidate))]
        protected GameScene _gameScene;
        [SerializeField]
        protected EnvironmentScriptable _environment;

        public GameScene gameScene => _gameScene;


        protected abstract bool IsValidate(GameScene scene);
    }
}