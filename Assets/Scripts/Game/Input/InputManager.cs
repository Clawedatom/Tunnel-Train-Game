using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Class Refeneces
    private static InputManager _instance;
    PlayerControls playerControls;
    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Player Fields")]
    private Vector2 movementInput;

    #endregion

    #region Properties
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputManager>();
                if (_instance == null)
                {
                    Debug.Log("Input handler has not been assigned");
                }
            }
            return _instance;
        }
    }
    public float Horizontal => movementInput.x;
    public float Vertical => movementInput.y;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        
    }
    public void OnStart()
    {
        GamePhaseManager.Instance.tunnelPhase.onStart += EnableMovement;
        GamePhaseManager.Instance.tunnelPhase.onEnd += DisableMovement;
    }
    #endregion

    #region Class Methods
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Movement.Rotate.performed += OnMovePerformed;
           
            playerControls.UI.Pause.performed += OnPausePerformed;

        }
        playerControls.Enable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    private void OnTunnelCheckPerformed(InputAction.CallbackContext ctx) // TEST
    {
        //test
        //TileManager.Instance.HandleTunnelCheck();
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.TogglePause();
    }

    private void OnDestroy()
    {
        GamePhaseManager.Instance.tunnelPhase.onStart -= EnableMovement;
        GamePhaseManager.Instance.tunnelPhase.onEnd -= DisableMovement;

    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Movement.Rotate.performed -= OnMovePerformed;
            playerControls.Test.TunnelCheck.performed -= OnTunnelCheckPerformed;
            playerControls.UI.Pause.performed -= OnPausePerformed;
            playerControls.Disable();
        }
    }

    public void EnableMovement()
    {
        if (playerManager == null) playerManager = PlayerManager.Instance;
        playerControls.Movement.Enable();
        playerManager.ToggleCanMove(true);
        
    }
    public void DisableMovement()
    {
        if (playerManager == null) playerManager = PlayerManager.Instance;
        playerControls.Movement.Disable();
        playerManager.ToggleCanMove(false);
    }
    #endregion

    #region Update methods

    #endregion
}
