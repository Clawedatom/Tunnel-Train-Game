using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Class References
    private static GameManager _instance;

    TileManager tileManager;
    CameraHandler cameraHandler;
    PlayerManager playerManager;
    InputManager inputManager;
    TrainManager trainManager;
    PlayerUIManager playerUIManager;
    #endregion

    #region Private Fields
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private bool isPaused;
    [SerializeField] private bool gameStarted;
    [SerializeField] private float gameTimer;
    #endregion

    #region Properties
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("GameManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public bool IsPaused
    {
        get { return isPaused; }
        set
        {
            isPaused = value;
            OnPauseToggle(); 
        }
    }

    public bool GameStarted
    {
        get { return gameStarted; }
        set { gameStarted = value; }
    }

    public float GetGameTimer => gameTimer;
    #endregion

    #region Start UP
    private void Awake()
    {
        GamePhaseManager.Instance.OnAwake();

        AssignClasses();
        AwakenClasses();
        gameStarted = false;
    }

    private void AssignClasses()
    {
        tileManager = TileManager.Instance;
        cameraHandler = CameraHandler.Instance;
       
        inputManager = InputManager.Instance;
        trainManager = TrainManager.Instance;

        playerUIManager = PlayerUIManager.Instance;
    }

    private void AwakenClasses()
    {
        tileManager.OnAwake();
        cameraHandler.OnAwake();
        
        inputManager.OnAwake();
        trainManager.OnAwake();
        playerUIManager.OnAwake();
    }

    private void Start()
    {
        StartClasses();

        //set up game
        SetUpGame();
        tileManager.SetPM();
        cameraHandler.SetPM();
        playerUIManager.SetPM();
        //start tunnel phase

        StartGame();
    }

    private void StartClasses()
    {
        tileManager.OnStart();
        cameraHandler.OnStart();
        
        inputManager.OnStart();
        trainManager.OnStart();
        playerUIManager.OnStart();
    }
    #endregion

    #region Class methods
    public void SetUpGame()
    {
        //MAP
        tileManager.SetUpGameMap();
        
        
        //Player
        SetUpPlayer();

        //Camera
        cameraHandler.SetUpCamera();

       
    }
    
    private void StartGame()
    {
        inputManager.DisableMovement();
        playerUIManager.HandleUpdateUIState(UIState.Tutorial);
       
    }

    private void SetUpPlayer()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
        playerManager = PlayerManager.Instance;

        playerManager.OnAwake();
        playerManager.OnStart();

        playerManager.TeleportPlayer(tileManager.GetStartPos());

    }
    #endregion

    #region Update Methods
    private void Update()
    {
        if (isPaused) return;
        cameraHandler.OnUpdate();
        playerManager.OnUpdate(inputManager.Horizontal, inputManager.Vertical);
        trainManager.OnUpdate();
        playerUIManager.OnUpdate();
        gameTimer += Time.deltaTime;
    }
    #endregion

    #region Game Loop
    public void OnTunnelPhaseStart()
    {
        GamePhaseManager.Instance.Phase_StartTunnelPhase();
        GameStarted = true;
    }
    public void OnTunnelPhaseEnd() // reach finsih zone
    {
        GamePhaseManager.Instance.Phase_EndTunnelPhase();
           
        
    }

    public void OnTrainPhaseStart()
    {
        GamePhaseManager.Instance.Phase_StartTrainPhase();
       
        
        
    }

    public void OnTrainPhaseEnd()
    {
        GamePhaseManager.Instance.Phase_EndTrainPhase();
    }
    public void OnTrainPhaseFail()
    {
        GamePhaseManager.Instance.Phase_FailTrainPhase();
    }
    public void OnTunnelPhaseFail()
    {
        GamePhaseManager.Instance.Phase_FailTunnelPhase();
    }

    public void PauseGame()
    {
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }

    public void OnPauseToggle()
    {
        if (IsPaused)
        {
            inputManager.DisableMovement();
            playerUIManager.HandleUpdateUIState(UIState.Pause);
        }
        else
        {
            inputManager.EnableMovement();
            playerUIManager.HandleUpdateUIState(UIState.InGame);
        }
    }

    public void TogglePause()
    {
        if (playerUIManager.ActiveState == UIState.Tutorial) return;
        IsPaused = !IsPaused;
    }

    public void OnPlayerDeath()
    {
        StartCoroutine(DestroyPlayerAndStartTunnelFail());
    }

    private IEnumerator DestroyPlayerAndStartTunnelFail()
    {
        tileManager.CreateExplosionAtPos(playerManager.transform.position);
        yield return new WaitForSeconds(2.0f);
        
        OnTunnelPhaseFail();
    }

    #endregion

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
