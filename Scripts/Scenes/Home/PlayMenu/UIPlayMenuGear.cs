
namespace Studio.Game
{
    public class UIPlayMenuGear : UIPlayMenu
    {
        protected override void GotoStage() => Popup<UIPopupSelectGearDungeon>.instance.OnPopup();
    }
}