using UnityEngine;

public class TunnelCompleteUI : BaseUI
{
    
    #region Class References

    #endregion

    #region Private Fields

    #endregion


    #region Properties

    #endregion


    #region Start Up

    #endregion

    #region Class Methods

    #endregion

    #region Buttons
    public void Button_StartTrain()
    {
        GameManager.Instance.OnTrainPhaseStart();
    }
    #endregion
}
