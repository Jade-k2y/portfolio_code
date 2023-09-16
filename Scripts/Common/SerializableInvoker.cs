using UnityEngine;
using UnityEngine.Events;
using System;


namespace Studio.Game
{
    [Serializable]
    public class SerializableInvoker<T>
    {
        [SerializeField]
        private T _value;
        private UnityAction<T> _event;

        public T value
        {
            get => _value;
            set
            {
                _value = value;
                _event?.Invoke(value);
            }
        }
        public event UnityAction<T> onEvent
        {
            add => _event += value;
            remove => _event -= value;
        }

        public void Default() => _value = default;
    }
}