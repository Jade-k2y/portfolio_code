using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Studio.Game
{
    public class UIScenePlay<T> : UIScene where T : StageScriptable
    {
        [SerializeField]
        protected T _scriptable;
        [SerializeField]
        protected UIStartScreen _screen;
        [SerializeField]
        protected UIPhaseProgress _stageInfo;
        [SerializeField]
        protected Button _pause;
        [SerializeField]
        protected UIPlayerCombatCard[] _cards;
        [SerializeField]
        protected UIHud _playerHud, _opponentHud;

        public UIHud playerHud => _playerHud;
        public UIHud opponentHud => _opponentHud;


        protected override void Start()
        {
            base.Start();

            _pause.SetClickEvent(() =>
            {
                if (_scriptable)
                {
                    Popup<UIPopupStagePause>.instance.OnPopup(new UIPopupStagePause.Model(_scriptable.gameScene, _scriptable.SetTitle));
                }
                else
                {
                    Popup<UIPopupStagePause>.instance.OnPopup();
                }
            });

            if (_scriptable)
            {
                if (_screen)
                {
                    _scriptable.SetTitle(_screen.title);
                }

                _scriptable.SetTitle(_stageInfo?.title);

                if (_scriptable is IPhaseScriptable scriptable)
                {
                    scriptable.InitPhaseProgress(_stageInfo);
                }
            }
        }


        public IEnumerator LoadCombatCard(IDictionary<int, StageActorPresenter> players, IActorCollection collection)
        {
            var wfef = new WaitForEndOfFrame();
            var actor = default(ActorPlayerScriptable);
            var count = _cards?.Length;

            for (var i = 0; i < count; ++i)
            {
                if (_cards[i])
                {
                    if (players?.ContainsKey(i) ?? false)
                    {
                        if (collection?.TryGetActorPlayerScriptable(players[i].actorName, out actor) ?? false)
                        {
                            _cards[i].Construction(actor, false);
                            _cards[i].SetUseItemReceiver(players[i]);
                        }

                        players[i]?.RegisterActionPresenter(_cards[i]);
                    }
                    else
                    {
                        _cards[i].Construction(null, false);
                    }

                    yield return wfef;
                }
            }
        }


        public IEnumerator OnFadeoutScreen()
        {
            if (_screen)
            {
                yield return _screen.Fadeout();
            }
        }


        public void UpdatePhaseProgress(int phaseIndex)
        {
            if (_scriptable is IPhaseScriptable scriptable)
            {
                scriptable.UpdatePhaseProgress(_stageInfo, phaseIndex);
            }
        }
    }
}