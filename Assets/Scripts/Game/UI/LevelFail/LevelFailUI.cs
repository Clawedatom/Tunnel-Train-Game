using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailUI : BaseUI
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
    public void Button_Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void Button_ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
