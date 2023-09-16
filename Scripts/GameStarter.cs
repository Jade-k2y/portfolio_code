using UnityEngine;
using System.Collections;


namespace Studio.Game
{
    public class GameStarter : MonoBehaviour
    {
        private void Start() => StartCoroutine(Startup());


        private IEnumerator Startup()
        {
            yield return AssetScriptable.InitializeAsync();

            SceneSwitcher.GotoTitle();
        }
    }
}