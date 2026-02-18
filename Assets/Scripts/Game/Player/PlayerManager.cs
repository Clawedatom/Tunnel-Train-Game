using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Class References
    private static PlayerManager _instance;

    PlayerDrill playerDrill;
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    #endregion

    #region Private Fields
    [Header("Player BOols")]
    [SerializeField] private bool canMove;
    #endregion

    #region Properties
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerManager>();
                if (_instance == null)
                {
                    Debug.LogError("PlayerManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public PlayerStats GetStats => playerStats;

    public bool CanMove => canMove;
    #endregion

    #region Start Up
    public void OnAwake()
    {
        playerDrill = GetComponent<PlayerDrill>();
        playerStats = GetComponent<PlayerStats>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDrill.OnAwake();
        playerMovement.OnAwake();
    }

    public void OnStart()
    {
        playerDrill.OnStart();
        playerStats.OnStart();
        playerMovement.OnStart();

        
    }
    #endregion

    #region Class Methods
    public void TeleportPlayer(Vector3 pos)
    {
        transform.position = pos;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            
            Debug.Log("Tunnel Complete");
            GameManager.Instance.OnTunnelPhaseEnd();
        }
    }
    #endregion

    #region Update Methods
    public void OnUpdate(float h, float v)
    {
        if (!canMove) return;
        playerDrill.OnUpdate();
        playerMovement.OnUpdate(h, v, true);
    }

    public void ToggleCanMove(bool state)
    {
        canMove = state;
        if (!canMove)
        {
            playerMovement.Stop();
        }
    }
    #endregion

    #region Stats
    public void TakeRandomDamage()
    {
        DamageType type = (DamageType)Random.Range(0, System.Enum.GetValues(typeof(DamageType)).Length);
        
        switch (type)
        {
            case DamageType.hull:
            {
                //double damage
                playerStats.DamageHull();
                break;
            }
            case DamageType.engine:
            {
                //speed
                playerStats.DamageEngine();
                break;
            }
            case DamageType.drill:
            {
                //damage
                playerStats.DamageDrill();
                break;
            }
            case DamageType.rotary:
            {
                //rotation
                playerStats.DamageRotary();
                break;
            }

        }

    }
    public void Stats_TakeDrillDamage()
    {
        playerStats.DamageDrill();
    }
    #endregion
}
