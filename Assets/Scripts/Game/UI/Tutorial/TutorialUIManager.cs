using UnityEngine;

public class TutorialUIManager : BaseUI
{
    #region Class References

    #endregion

    #region Private Fields
    private int index;
    [Tooltip("Order of player seeing")][SerializeField] private GameObject[] tutorialScreens; // MAKE SURE IN ORDER
    #endregion


    #region Properties

    #endregion


    #region Start Up
    public override void OnAwake()
    {
        index = 0;
        if (tutorialScreens.Length == 0)
        {
            Debug.LogError("Assign tutorial screens");
        }
    }
    #endregion

    #region Class Methods

    public override void OnEnableUI()
    {
        base.OnEnableUI();

        InputManager.Instance.DisableMovement();

        index = 0;
        UpdateUI();
    }

    
    public void Button_ArrowPrev()
    {
        index--;
        UpdateUI();

    }
    public void Button_ArrowNext()
    {
        index++;
        UpdateUI();
    }

    public void Button_CloseTutorial()
    {
        if (GameManager.Instance.GameStarted)
        {
            PlayerUIManager.Instance.HandleUpdateUIState(UIState.InGame);
            GameManager.Instance.TogglePause();
            return;
        }
        GameManager.Instance.OnTunnelPhaseStart();
    }

    private void UpdateUI()
    {
        if (index > tutorialScreens.Length - 1) index = 0; //wwrap around
        if (index < 0) index = tutorialScreens.Length - 1;

        foreach(GameObject tutorialScreen in tutorialScreens)
        {
            tutorialScreen.SetActive(false);
        }
        tutorialScreens[index].SetActive(true);

    }

    #endregion
}
