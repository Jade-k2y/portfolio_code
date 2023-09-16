using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    public class UICard
    {
        #region empty
        [Serializable]
        public class Empty
        {
            public CanvasGroup canvasGroup;
            public Button button;

            public void SetActive(bool active)
            {
                if (active)
                {
                    canvasGroup.Show();
                }
                else
                {
                    canvasGroup.Hide();
                }
            }
        }
        #endregion

        #region active
        [Serializable]
        public class Active : Empty
        {
            public Graphic background;
            public Image mastery, thumbnail, eyeBG;
            public TMP_Text actorName, level;
        }
        #endregion
    }


    public abstract class UICard<TEmpty, TActive> : MonoBehaviour
        where TEmpty : UICard.Empty
        where TActive : UICard.Active
    {
        [SerializeField]
        protected CanvasGroup _canvasGroup;
        [SerializeField]
        protected TEmpty _empty;
        [SerializeField]
        protected TActive _active;
        [SerializeField]
        protected Image _eyeBlock, _focus;
        [SerializeField]
        protected MasteryScriptable.AssetReference _mastery;

        private ActorPlayerScriptable _scriptable;
        private MasteryScriptable _masteryAsset;
        protected UIDraggableCard _draggable;
        protected UIDroppableCard _droppable;
        protected bool _activated;

        public ActorPlayerScriptable scriptable
        {
            get => _scriptable;
            set
            {
                if ((_scriptable = value) && masteryAsset)
                {
                    masteryAsset.SetMasteryIcon(_active?.mastery, _scriptable.mastery);
                }
            }
        }
        public MasteryScriptable masteryAsset => AssetScriptable.GetLoadAsset(_mastery, ref _masteryAsset);


        protected virtual void OnDestroy()
        {
            if (_masteryAsset)
            {
                AssetScriptable.ReleaseAsset(_mastery);
            }
        }


        protected virtual void Awake()
        {
            _draggable = GetComponent<UIDraggableCard>();
            _droppable = GetComponent<UIDroppableCard>();
        }


        protected virtual void Start()
        {
            SetActiveFocus(false);
            /*
#if UNITY_EDITOR
            _active?.button.SetClickEvent(() => Popup<UIPopupActorPlayer>.instance.OnPopup(), false);
#endif
            */
        }


        public abstract void Construction(ActorPlayerScriptable scriptable, bool register);


        public void SetActiveLayer(bool active)
        {
            _activated = active;
            _active?.SetActive(_activated);
            _empty?.SetActive(!_activated);
            /*
            SetEnableDraggableCard(_activated);
            SetEnableDroppableCard(!_activated);
            */
            SetInteractableActiveLayer(_activated);
            SetInteractableEmptyLayer(false);
        }


        public void SetActiveFocus(bool active) => SetEnableBehaviour(_focus, active);

        protected void SetActiveEyeBlock(bool enable) => SetEnableBehaviour(_eyeBlock, enable);

        public virtual void SetEnableDraggableCard(bool enable) => SetEnableBehaviour(_draggable, enable);

        public virtual void SetEnableDroppableCard(bool enable) => SetEnableBehaviour(_droppable, enable);

        private void SetEnableBehaviour(Behaviour behaviour, bool enable)
        {
            if (behaviour)
            {
                behaviour.enabled = enable;
            }
        }


        public void SetInteractableActiveLayer(bool interactable) => SetInteractableButton(_active?.button, interactable);

        public void SetInteractableEmptyLayer(bool interactable) => SetInteractableButton(_empty?.button, interactable);

        private void SetInteractableButton(Button button, bool interactable)
        {
            if (button)
            {
                button.interactable = interactable;
            }
        }
    }
}