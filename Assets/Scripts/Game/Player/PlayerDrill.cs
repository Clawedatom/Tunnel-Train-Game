
using UnityEngine;

public class PlayerDrill : MonoBehaviour
{
    #region Class References
    PlayerManager playerManager;
    TileManager  tileManager;
    #endregion

    #region Private Fields
    [Header("Drill fields")]
    [SerializeField] private Transform drillTipTransform;
    [SerializeField] private int drillSize = 2; // loop between drilltIp -> grid indexx between index - drillsize and inedx + drillsize for bigger drilling
    
    [SerializeField] private bool canDrill;

    [Header("Sound")]
    [SerializeField] private float soundTimer = 0.0f;
    private float soundCooldown = 0.6f;
    [SerializeField] private AudioClip drillSound;
    #endregion

    #region Properties

    #endregion


    #region Start Up
    public void OnAwake()
    {
        tileManager = TileManager.Instance;
        playerManager = PlayerManager.Instance;
    }

    public void OnStart()
    {
        canDrill = true;
    }
    #endregion

    #region Class Methods

    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        if (canDrill) HandleDrill();
    }

    private void HandleDrill()
    {
        //convert drill world to tile grid idnex
        Vector2Int gridIndex = tileManager.WorldToGrid(drillTipTransform.position);

        tileManager.DrillAtIndex(gridIndex, drillSize, playerManager.GetStats.GetCurrentDrillDamage);
        
        //get tile at gridindex
        //display that particle

        TileData targetTD = tileManager.TileDataFromWorldPos(drillTipTransform.position);

        if (targetTD != null)
        {
            ParticleSystem p = Instantiate(targetTD.DrillParticle, drillTipTransform);

            p.Play();
            Destroy(p.gameObject, p.main.duration + p.main.startLifetime.constantMax);
        }


        soundTimer += Time.deltaTime;
        if (soundTimer > soundCooldown)
        {
            soundTimer = 0.0f;
            SoundManager.Instance.PlaySoundAtPos(drillSound, transform.position);
        }

    }
    #endregion
}
