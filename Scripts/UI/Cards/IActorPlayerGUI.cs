using UnityEngine.UI;
using TMPro;


namespace Studio.Game
{
    public interface IActorPlayerGUI
    {
        Graphic background { get; }
        Image mastery { get; }
        Image thumbnail { get; }
        TMP_Text actorName { get; }
    }
}