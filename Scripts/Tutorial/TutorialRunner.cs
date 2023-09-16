using UnityEngine;
using System.Collections;
using UniRx;
using CustomInspector;


namespace Studio.Game
{
    public enum TutorialType
    {
        MoveMenu,
        ProfileCircle,
        ProfileBar,
        Setting,
        CharacterUnlock,
        CharacterLocation,
        CharacterChange,
        CharacterRemove,
        UseSpirit
    }


    public interface ITutorial
    {
        bool useTrigger { get; }
        int order { get; }
        bool isDone { get; }
        IEnumerator Play();
    }


    public interface ITutorialMessage
    {
        TutorialType type { get; }
    }


    public class TutorialRunner : MonoBehaviour, ITutorial
    {
        [SerializeField]
        private TutorialType _type;
        [SerializeField]
        private bool _useTrigger;
        [SerializeField, ShowIfNot(nameof(_useTrigger))]
        private int _order;
        [SerializeField]
        private float _delay;
        [SerializeField]
        private TutorialGuide _guide;

        private bool _isPlaying;

        bool ITutorial.useTrigger => _useTrigger;
        int ITutorial.order => _order;
        public bool isDone => Global<User>.instance.IsDoneTutorial(_type);


        private void Start()
        {
            if (!isDone)
            {
                MessageBroker.Default.Receive<TutorialType>()
                    .Subscribe(OnMessageTutorial)
                    .AddTo(this);
            }
        }


        public IEnumerator Play()
        {
            if (!isDone && !_isPlaying)
            {
                if (0 < _delay)
                {
                    var wfs = new WaitForSeconds(_delay);

                    yield return wfs;
                }

                _isPlaying = true;

                Popup<UIPopupGuide>.instance.OnPopup(new UIPopupGuide.Model(_guide, () => _isPlaying = false));
                yield return new WaitUntil(() => !_isPlaying);
            }

            Global<User>.instance.CompleteTutorial(_type);
        }


        private void OnMessageTutorial(TutorialType messageType)
        {
            if (!isDone && _type == messageType)
            {
                StartCoroutine((this as ITutorial).Play());
            }
        }
    }
}