using UnityEngine;
using CustomInspector;


namespace Studio.Game
{
    public class GearNameScriptable : ScriptableObject
    {
        [SerializeField, Tab("MODIFIER")]
        public string[] _modifiers;
        [SerializeField, Tab("MODIFICAND")]
        public string[] _modificands;
    }
}