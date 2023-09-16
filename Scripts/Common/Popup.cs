using UnityEngine;
using UnityEngine.AddressableAssets;
using System;


namespace Studio.Game
{
    public static class Popup<T> where T : UIPopup
    {
        static Popup() => _attribute = Attribute.GetCustomAttribute(typeof(T), typeof(PopupAttribute)) as PopupAttribute;

        private static readonly PopupAttribute _attribute;
        private static T _instance;

        public static T instance
        {
            get
            {
                if (!_instance)
                {
                    var handle = Addressables.LoadAssetAsync<GameObject>(_attribute.label);
                    var asset = handle.WaitForCompletion();
                    var instantiate = InstanceUI.CallbackAddChildren?.Invoke(asset);

                    if (instantiate)
                    {
                        _instance = instantiate.GetComponent<T>();
                    }

                    Addressables.Release(handle);
                }

                return _instance;
            }
        }
    }
}