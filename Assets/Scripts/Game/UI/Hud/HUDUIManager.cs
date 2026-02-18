using TMPro;
using UnityEngine;

public class HUDUIManager : BaseUI
{
    #region Class References
    PlayerStats stats;
    #endregion

    #region Private Fields
    [Header("Text ")]
    [SerializeField] private TMP_Text drillText;
    [SerializeField] private TMP_Text engineText;
    [SerializeField] private TMP_Text rotaryText;
    [SerializeField] private TMP_Text hullText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text distanceText;
    #endregion


    #region Properties

    #endregion


    #region Start Up
    public void AssignPs()
    {
        stats = PlayerManager.Instance.GetStats;
    }
    #endregion

    #region Class Methods
    public void OnUpdate()
    {
        UpdateText();
    }
    private void UpdateText()
    {
        // Drill %
        float drillPerc = stats.DrillIntegrity * 100f;
        drillText.text = (int)drillPerc + "%";

        // Engine %
        float enginePerc = stats.EngineIntegrity * 100f;
        engineText.text = (int)enginePerc + "%";

        // Rotary %
        float rotaryPerc = stats.RotaryIntegrity * 100f;
        rotaryText.text = (int)rotaryPerc + "%";

        // Hull %
        float hullPerc = stats.HullIntegrity * 100f;
        hullText.text = (int)hullPerc + "%";

        // Timer
        timerText.text = Mathf.RoundToInt(GameManager.Instance.GetGameTimer) + "s";

        // Distance from end
        distanceText.text = Mathf.RoundToInt(stats.GetDistFromEnd()) + "m";

    }
    #endregion
}
