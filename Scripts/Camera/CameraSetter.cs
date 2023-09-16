using UnityEngine;
using UnityEngine.SceneManagement;


namespace Studio.Game
{
    [RequireComponent(typeof(Camera))]
    public class CameraSetter : MonoBehaviour
    {
        [SerializeField]
        private CameraScriptable _scriptable;


        private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;


        private void OnEnable() => SceneManager.activeSceneChanged += OnActiveSceneChanged;


        private void OnActiveSceneChanged(Scene prev, Scene next)
        {
            if (_scriptable)
            {
                _scriptable.OnActiveSceneChanged(prev, next);
            }
        }
    }
}