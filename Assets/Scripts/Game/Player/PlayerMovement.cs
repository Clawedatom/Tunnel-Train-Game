using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Class References
    TileManager tileManager;
    PlayerManager playerManager;
    Rigidbody2D rb;
    #endregion

    #region Private Fields
    [Header("Movement fields")]
    [SerializeField] private float xRot;
    
    
    #endregion

    #region Properties

    #endregion


    #region Start Up
    public void OnAwake()
    {
        rb = GetComponent<Rigidbody2D>();   
        playerManager = PlayerManager.Instance;
    }

    public void OnStart()
    {
        
    }
    #endregion

    #region Class Methods
    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
    }
    #endregion

    #region Update Methods
    public void OnUpdate(float h, float v, bool canMove)
    {
       if (!canMove) return;

        HandleRotation(h, v);
        HandleMovement();
    }
    private void HandleMovement()
    {
        Vector3 vel = transform.right * playerManager.GetStats.GetCurrentSpeed;
        rb.linearVelocity = vel;
    }

    private void HandleRotation(float h, float v)
    {
       

        xRot = -h * playerManager.GetStats.GetCurrentRotSpeed; // invert so "A" goes "up"

        transform.Rotate(new Vector3(0, 0, xRot * Time.deltaTime));
    }
    #endregion
}
