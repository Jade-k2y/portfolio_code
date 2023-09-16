using UnityEngine.EventSystems;
using Studio.Frameworks;
using UnityEditor;

namespace Studio.Game
{
    public interface IReplaceableCard
    {
        bool replaceable { get; }
        ActorPlayerScriptable scriptable { get; }

        void Construction(ActorPlayerScriptable replace, bool register);
    }


    public class UIDroppableCard : UIDroppable
    {
        private IReplaceableCard _to;


        private void Awake() => enabled = TryGetComponent(out _to);


        public override void OnDrop(PointerEventData eventData)
        {
            if (eventData?.pointerDrag && eventData.pointerDrag.TryGetComponent<IReplaceableCard>(out var from))
            {
                var scriptable = _to?.scriptable;

                if (from.scriptable.actorName != scriptable.actorName)
                {
                    _to?.Construction(from.scriptable, true);

                    if (from.replaceable)
                    {
                        from.Construction(scriptable, true);
                    }
                }

                if (eventData.pointerDrag.TryGetComponent<UIDraggableCard>(out var drag))
                {
                    drag.OnEndDrag(eventData);
                }
            }
        }
    }
}