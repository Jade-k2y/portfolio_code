using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;


namespace Studio.Game
{
    public abstract class UIPlayMenu : MonoBehaviour
    {
        [SerializeField]
        protected Button _play;
        [SerializeField]
        protected UnityEvent<UIMenu.Menu> _onGotoMenu;


        protected virtual void Start() => _play.SetClickEvent(OnClickPlay, false);


        protected virtual void OnClickPlay()
        {
            var startable = true;

            if (Global<User>.instance.TryGetTeamPlayers(out var team))
            {
                if (team.All(x => !x.isExist))
                {
                    Popup<UIPopupWarning>.instance.OnPopup(
                        new UIPopupWarning.Model(
                            "시작 불가",
                            "파티에 1명 이상의 캐릭터를 배치 하세요.",
                            () => _onGotoMenu?.Invoke(UIMenu.Menu.Character)));
                    startable = false;
                }
            }

            if (startable)
            {
                GotoStage();
            }
        }


        protected abstract void GotoStage();
    }
}