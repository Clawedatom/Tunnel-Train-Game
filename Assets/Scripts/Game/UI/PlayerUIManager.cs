using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    #region Class Referenecs
    private static PlayerUIManager _instance;

    HUDUIManager hudUIManager;
    LevelCompleteUI levelCompleteUI;
    TunnelCompleteUI tunnelCompleteUI;
    PauseUIManager pauseUIManager;
    TutorialUIManager tutorialUIManager;  
    LevelFailUI levelFailUI;
    #endregion

    #region Private Fields
    [Header("UI Fields")]
    [SerializeField] private UIState activeState;
    #endregion

    #region Properties
    public static PlayerUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerUIManager>();
                if (_instance == null)
                {
                    Debug.LogError("PlayerUIManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public UIState ActiveState => activeState;
    #endregion

    #region Start Up
    public void SetPM()
    {
        hudUIManager.AssignPs();
    }

    public void OnAwake()
    {
        SetUpUI();
    }
    private void SetUpUI()
    {
        //assign
        hudUIManager = GetComponentInChildren<HUDUIManager>();
        levelCompleteUI = GetComponentInChildren<LevelCompleteUI>();
        tunnelCompleteUI = GetComponentInChildren<TunnelCompleteUI>();
        pauseUIManager = GetComponentInChildren<PauseUIManager>();
        tutorialUIManager = GetComponentInChildren<TutorialUIManager>();
        levelFailUI = GetComponentInChildren<LevelFailUI>();
        //awake
        hudUIManager.OnAwake();
        levelCompleteUI.OnAwake();
        tunnelCompleteUI.OnAwake();
        pauseUIManager.OnAwake();
        tutorialUIManager.OnAwake();
        levelFailUI.OnAwake();

        
    }

    public void OnStart()
    {
        

        GamePhaseManager.Instance.tunnelPhase.onStart += OnTunnelPhaseStart;
        GamePhaseManager.Instance.tunnelPhase.onEnd += OnTunnelPhaseEnd;

        GamePhaseManager.Instance.trainPhase.onStart += OnTrainPhaseStart;
        GamePhaseManager.Instance.trainPhase.onEnd += OnTrainPhaseEnd;
        GamePhaseManager.Instance.trainPhase.onFail += OnTrainPhaseFail;

        GamePhaseManager.Instance.tunnelPhase.onFail += OnTunnelPhaseFail;
    
    
            //start
        hudUIManager.OnStart();
        levelCompleteUI.OnStart();
        tunnelCompleteUI.OnStart();
        pauseUIManager.OnStart();
        tutorialUIManager.OnStart();
        levelFailUI.OnStart();
    }

    #endregion

    #region Update
    public void OnUpdate()
    {
        hudUIManager.OnUpdate();
    }
    #endregion
    #region Class Methods
    public void HandleUpdateUIState(UIState newState)
    {
        DisableAllUI();

        activeState = newState;

        switch (activeState)
        {
            case UIState.InGame:
            EnableInGameUI();
            break;

            case UIState.TunnelComplete:
            EnableTunnelCompleteUI();
            break;

            case UIState.LevelComplete:
            EnableLevelCompleteUI();
            break;

            case UIState.Pause:
            EnablePauseUI();
            break;

            case UIState.Tutorial:
            EnableTutorialUI();
            break;

            case UIState.LevelFail:
            EnableLevelFail();
            break;
        }

    }
    #endregion

    #region UI States
    private void DisableAllUI()
    {
        hudUIManager.OnDisableUI();
        tunnelCompleteUI.OnDisableUI();
        levelCompleteUI?.OnDisableUI();
        pauseUIManager.OnDisableUI();
        tutorialUIManager.OnDisableUI();
        levelFailUI.OnDisableUI();
    }
    private void EnableInGameUI()
    {
        hudUIManager.OnEnableUI();
       
    }

    private void EnableTunnelCompleteUI()
    {
        tunnelCompleteUI.OnEnableUI();
    }

    private void EnableLevelCompleteUI()
    {
        levelCompleteUI.OnEnableUI();
    }
    private void EnablePauseUI()
    {
        pauseUIManager.OnEnableUI();
    }

    private void EnableTutorialUI()
    {
        tutorialUIManager.OnEnableUI();
    }

    private void EnableLevelFail()
    {
        levelFailUI.OnEnableUI();
    }
    #endregion

    

    #region Clean Up
    private void OnDestroy()
    {
        if (GamePhaseManager._instance != null)
        {
            GamePhaseManager.Instance.tunnelPhase.onStart -= OnTunnelPhaseStart;
            GamePhaseManager.Instance.tunnelPhase.onEnd -= OnTunnelPhaseEnd;
            GamePhaseManager.Instance.trainPhase.onStart -= OnTrainPhaseStart;
            GamePhaseManager.Instance.trainPhase.onEnd -= OnTrainPhaseEnd;
            GamePhaseManager.Instance.trainPhase.onFail -= OnTrainPhaseFail;
            GamePhaseManager.Instance.tunnelPhase.onFail -= OnTunnelPhaseFail;
        }

       
    }
    private void OnTunnelPhaseStart() => HandleUpdateUIState(UIState.InGame);
    private void OnTunnelPhaseEnd() => HandleUpdateUIState(UIState.TunnelComplete);
    private void OnTrainPhaseStart() => DisableAllUI();
    private void OnTrainPhaseEnd() => HandleUpdateUIState(UIState.LevelComplete);
    private void OnTrainPhaseFail() => HandleUpdateUIState(UIState.LevelFail);
    private void OnTunnelPhaseFail() => HandleUpdateUIState(UIState.LevelFail);
    #endregion
}
