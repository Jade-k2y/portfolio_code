using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using DG.Tweening;


namespace Studio.Game
{
    [CreateAssetMenu(menuName = "Crypto TF/Game/Formation/Scriptable")]
    public class FormationScriptable : ScriptableObject
    {
        [Serializable]
        public class AssetReference : AssetReferenceT<FormationScriptable>
        {
            public AssetReference(string guid) : base(guid) { }
        }

        [SerializeField]
        private Vector3[] _positions;

        public int count => _positions?.Length ?? 0;


        public void SetFormation(int index, Transform transform, Vector3 center)
        {
            if (transform && (0 <= index && index < _positions?.Length))
            {
                transform.position = center + _positions[index];
            }
        }


        public void MoveFormation(int index, Transform transform, Vector3 center, float duration, TweenCallback onArrived)
        {
            var target = center + _positions[index];

            if (transform && (0 <= index && index < _positions?.Length))
            {
                if (transform.position != target)
                {
                    transform.DOMove(target, duration).OnComplete(onArrived);
                }
                else
                {
                    onArrived?.Invoke();
                }
            }
        }
    }
}