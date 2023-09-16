
namespace Studio.Game
{
    public class UIPlayMenuPvPLeague : UIPlayMenu
    {
        protected override void GotoStage() => SceneContext.GotoContent(GameScene.PVP_STAGE);
    }
}