using UnityEngine;
using System;
using CustomInspector;


namespace Studio.Game
{
    [Serializable]
    public class TutorialGuide
    {
        [SerializeField]
        private bool _hold;
        [SerializeField]
        private float _y;
        [SerializeField, TextArea]
        private string _message;
        [SerializeField]
        private bool _isFocus;
        [SerializeField, ShowIf(nameof(_isFocus))]
        private float _rate = 1f;
        [SerializeField, ShowIf(nameof(_isFocus))]
        private Vector3 _focus;
        [SerializeField, ShowIf(nameof(_isFocus))]
        private Transform _focusTransform;
        [SerializeField]
        private bool _tweening;
        [SerializeField, ShowIf(nameof(_tweening))]
        private float _duration = 2f;
        [SerializeField, ShowIf(nameof(_tweening))]
        private Vector3 _focusMove;
        [SerializeField, ShowIf(nameof(_tweening))]
        private Transform _focusMoveTransform;

        public bool hold => _hold;
        public float y => _y;
        public string message => _message;
        public bool isFocus => _isFocus;
        public bool tweening => _tweening;
        public Vector3 focus => _focus;
        public Vector3 focusMove => _focusMove;
        public Transform focusTransform => _focusTransform;
        public Transform focusMoveTransform => _focusMoveTransform;
        public float rate => _rate;
        public float duration => _duration;


        public TutorialGuide SetHold(bool hold)
        {
            _hold = hold;

            return this;
        }

        public TutorialGuide SetPositionY(float y)
        {
            _y = y;

            return this;
        }

        public TutorialGuide SetMessage(string message)
        {
            _message = message;

            return this;
        }

        public TutorialGuide SetFocus(bool focus)
        {
            _isFocus = focus;

            return this;
        }

        public TutorialGuide SetFocusPosition(Vector3 pos)
        {
            _focus = pos;

            return this;
        }

        public TutorialGuide SetFocusMovePosition(Vector3 pos)
        {
            _focusMove = pos;
            _tweening = true;

            return this;
        }

        public TutorialGuide SetFocusTransform(Transform transform)
        {
            _focusTransform = transform;

            return this;
        }

        public TutorialGuide SetFocusMoveTransform(Transform transform)
        {
            if (transform)
            {
                _focusMoveTransform = transform;
                _tweening = true;
            }

            return this;
        }

        public TutorialGuide SetScaleRate(float rate)
        {
            _rate = rate;

            return this;
        }

        public TutorialGuide SetDuration(float duration)
        {
            _duration = duration;

            return this;
        }
    }
}