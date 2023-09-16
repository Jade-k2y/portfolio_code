using UnityEngine;
using CustomInspector;


namespace Studio.Game
{
    public interface IParallaxRenderer
    {
        void SetViewshed(Transform viewshed);
        void SetSpriteLayer(float weight, int sortingOrder, Sprite sprite);
    }


    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxBackground : MonoBehaviour, IParallaxRenderer
    {
        [SerializeField, ReadOnly]
        private Transform _viewshed;
        [SerializeField, ReadOnly]
        private float _weight;
        [SerializeField]
        private SpriteRenderer _root;
        [SerializeField]
        private SpriteRenderer[] _children;

        private float _start, _length, _distance, _inverse;


        private void Start()
        {
            _start = transform.position.x;

            if (_root)
            {
                _length = _root.bounds.size.x;
            }
        }


        private void FixedUpdate()
        {
            if (_viewshed)
            {
                _distance = _viewshed.position.x * _weight;
                _inverse = _viewshed.position.x * (1f - _weight);

                if (_inverse > (_start + _length))
                {
                    _start += _length;
                }
                else if (_inverse < (_start - _length))
                {
                    _start -= _length;
                }

                transform.position = new Vector3(_start + _distance, transform.position.y, transform.position.z);
            }
        }


        void IParallaxRenderer.SetViewshed(Transform viewshed)
        {
            _viewshed = viewshed;
        }


        void IParallaxRenderer.SetSpriteLayer(float weight, int sortingOrder, Sprite sprite)
        {
            _weight = weight;

            if (_root)
            {
                _root.sortingOrder = sortingOrder;
                _root.sprite = sprite;
                _root.enabled = _root.sprite;
            }

            var count = _children?.Length ?? 0;

            for (var i = 0; i < count; ++i)
            {
                if (_children[i])
                {
                    _children[i].sortingOrder = sortingOrder;
                    _children[i].sprite = sprite;
                    _children[i].enabled = _children[i].sprite;
                }
            }
        }
    }
}

