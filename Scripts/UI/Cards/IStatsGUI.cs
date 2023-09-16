using TMPro;


namespace Studio.Game
{
    public interface IStatsGUI
    {
        TMP_Text hp { get; }
        TMP_Text atk { get; }
        TMP_Text def { get; }
        TMP_Text @int { get; }
        TMP_Text res { get; }
        TMP_Text spd { get; }
        TMP_Text crt { get; }
        TMP_Text crd { get; }
    }
}