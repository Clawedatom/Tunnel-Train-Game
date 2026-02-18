using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIManager : BaseUI
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
    public override void OnEnableUI()
    {
        base.OnEnableUI();
        
    }

    public void Button_Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void Button_Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void Button_OpenTutorial()
    {
        PlayerUIManager.Instance.HandleUpdateUIState(UIState.Tutorial);
    }
    public void Button_ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
