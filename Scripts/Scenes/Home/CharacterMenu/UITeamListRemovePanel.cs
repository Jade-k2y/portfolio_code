using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Studio.Frameworks;


namespace Studio.Game
{
    public class UITeamListRemovePanel : UIDroppable
    {
        [SerializeField]
        private UnityEvent<UIPlayerCard> _eventLeaveTeam;

        private ActorPlayerScriptable _empty;


        public void SetEmptyScriptable(ActorPlayerScriptable empty) => _empty = empty;


        public override void OnDrop(PointerEventData eventData)
        {
            if (eventData?.pointerDrag && eventData.pointerDrag.TryGetComponent<UIPlayerCard>(out var card))
            {
                card.Construction(_empty, true);

                _eventLeaveTeam?.Invoke(card);

                if (eventData.pointerDrag.TryGetComponent<UIDraggableCard>(out var drag))
                {
                    drag.OnEndDrag(eventData);
                }
            }
        }
    }
}