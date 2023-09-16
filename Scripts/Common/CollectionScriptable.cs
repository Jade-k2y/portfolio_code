using UnityEngine;
using UnityEngine.AddressableAssets;
using System;


namespace Studio.Game
{
    public abstract class CollectionScriptable<T, R> : ScriptableObject
        where T : ScriptableObject
        where R : AssetReference
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<T>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [Serializable]
        public class Element
        {
            [SerializeField]
            protected string _name;
            [SerializeField]
            protected R _reference;
            [SerializeField]
            protected Keyword _keyword;

            public string name => _name;
            public R reference => _reference;
        }

        [SerializeField]
        protected Element[] _collection;


        public R GetRandomElementAssetReference()
        {
            var count = _collection?.Length ?? 0;

            if (0 < count)
            {
                return _collection[UnityEngine.Random.Range(0, count)].reference;
            }

            return default;
        }
    }
}