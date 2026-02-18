using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Class References
    private static CameraHandler _instance;

    PlayerManager playerManager;
    #endregion

    #region Private Fields
    [Header("Camera fields")]
    Transform target;
    [SerializeField] private float followSpeed = 0.5f;
    #endregion

    #region Properties
    public static CameraHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CameraHandler>();
                if (_instance == null)
                {
                    Debug.LogError("CameraHandler has not been assigned");
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Start Up
    public void OnAwake()
    {
        
    }

    public void OnStart()
    {
       
    }

    public void SetPM()
    {
        if (playerManager == null)
        {
            playerManager = PlayerManager.Instance;
        }
    }
    #endregion

    #region Class Methods
    public void SetUpCamera()
    {
        playerManager = PlayerManager.Instance;
        SetTarget(playerManager.transform);
    }
    public void SnapCamToTarget()
    {
        transform.position = target.transform.position; // snap to at start
    }
    private void HandleFollowPlayer()
    {
        if (target == null) return;     
        Vector3 targetPos = target.transform.position;

        Vector3 lerpPos = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        transform.position = lerpPos;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        SnapCamToTarget();
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        HandleFollowPlayer();
    }
    #endregion
}
