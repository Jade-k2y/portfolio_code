using UnityEngine;
using System;
using AssetKits.ParticleImage;
using CustomInspector;
using ParticlePlayMode = AssetKits.ParticleImage.Enumerations.PlayMode;


namespace Studio.Game
{
    [RequireComponent(typeof(ParticleImage))]
    public class UIAttractor : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        protected ParticleImage _particle;
        [SerializeField]
        protected float _maxRatePserSecond;

        public int count => _particle ? _particle.particles.Count : 0;


        private void OnValidate()
        {
            if (!_particle)
            {
                _particle = GetComponent<ParticleImage>();
            }

            if (_particle)
            {
                _particle.PlayMode = ParticlePlayMode.None;
                _particle.rateOverTime = _maxRatePserSecond;
            }
        }


        private void Start() => SetRateOverTime(_maxRatePserSecond);


        public UIAttractor SetTarget(Transform target)
        {
            if (_particle)
            {
                _particle.attractorTarget = target;
            }

            return this;
        }


        public UIAttractor SetRateOverTime(float rateOverTime)
        {
            if (_particle)
            {
                _particle.rateOverTime = _maxRatePserSecond * Mathf.Clamp(rateOverTime, 0f, 1f);
            }

            return this;
        }


        public UIAttractor SetPieceArrived(Action onArrived)
        {
            if (_particle)
            {
                _particle.onParticleFinish.RemoveAllListeners();
                _particle.onParticleFinish.AddListener(() => onArrived?.Invoke());
            }

            return this;
        }


        public void Play()
        {
            if (_particle)
            {
                _particle.Play();
            }
        }
    }
}