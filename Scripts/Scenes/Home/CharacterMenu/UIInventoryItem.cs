using UnityEngine;
using UnityEngine.Events;


namespace Studio.Game
{
    public class UIInventoryItem : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<int> _onToggled;

        private int _index;


        private void Start() => _index = transform.GetSiblingIndex();


        public void OnToggleChanged(bool isOn)
        {
            if (isOn)
            {
                _onToggled?.Invoke(_index);
            }
        }
    }
}