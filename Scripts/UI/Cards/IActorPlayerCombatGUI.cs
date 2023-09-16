using UnityEngine.UI;
using System;
using TMPro;


namespace Studio.Game
{
    #region stats gui
    [Serializable]
    public class StatsGUI
    {
        public TMP_Text value;
        public Image bar;
    }
    #endregion


    public interface IActorPlayerCombatGUI : IActorPlayerGUI
    {
        StatsGUI hp { get; }
        StatsGUI sp { get; }
    }
}