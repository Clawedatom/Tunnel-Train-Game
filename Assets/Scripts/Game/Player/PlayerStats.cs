using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    #region Class References

    #endregion

    #region Private Fields
    [Header("Health Fields")]
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthDecreaseVal = 25.0f;

    [Header("Drill Damage Fields")]
    [SerializeField] private float maxDrillDmg = 15f;
    [SerializeField] private float currentDrillDmg;
    [SerializeField] private float drillDecreaseAmt = 0.5f;

    [Header("Rotation  Fields")]
    [SerializeField] private float maxRotSpeed = 35f;
    [SerializeField] private float currentRotSpeed;
    [SerializeField] private float rotDecreaseAmt = 0.5f;

    [Header("Engine  Fields")]
    [SerializeField] private float maxSpeed = 15;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float speedDecreaseAmount = 0.5f;
    #endregion


    #region Properties
    public float GetCurrentHealth => currentHealth;
    public float GetCurrentDrillDamage => currentDrillDmg;
    public float GetCurrentRotSpeed => currentRotSpeed;
    public float GetCurrentSpeed => currentSpeed;

    public float DrillIntegrity => currentDrillDmg / maxDrillDmg;
    public float HullIntegrity => currentHealth / maxHealth;
    public float EngineIntegrity => currentSpeed / maxSpeed;
    public float RotaryIntegrity => currentRotSpeed / maxRotSpeed;

    public float GetDistFromEnd()
    {
        return TileManager.Instance.GetDistToEnd(transform.position);
    }
    #endregion


    #region Start Up
    public void OnAwake()
    {

    }

    public void OnStart()
    {
        currentHealth = maxHealth;
        currentDrillDmg = maxDrillDmg;
        currentRotSpeed = maxRotSpeed;
        currentSpeed = maxSpeed;
    }
    #endregion

    #region Class Methods
   
   

    public void DamageDrill() => ApplyDamage(DamageType.drill, drillDecreaseAmt);
    public void DamageEngine() => ApplyDamage(DamageType.engine, speedDecreaseAmount);
    public void DamageHull() => ApplyDamage(DamageType.hull, healthDecreaseVal);
    public void DamageRotary() => ApplyDamage(DamageType.rotary, rotDecreaseAmt);
    private void ApplyDamage(DamageType type, float amount)
    {

        Debug.Log("Rot Speed: " + currentRotSpeed + " Removing +" + amount);
        switch (type)
        {
            case DamageType.hull:
            {
                currentHealth -= amount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                
                if (currentHealth <= 0.5f)
                {
                    GameManager.Instance.OnPlayerDeath();
                }
                break;
            }

            case DamageType.engine:
            {
                currentSpeed -= amount;
                currentSpeed = Mathf.Clamp(currentSpeed, maxSpeed / 4f, maxSpeed);
            
                break;

            }

            case DamageType.drill:
            {
                currentDrillDmg -= amount;
                //Debug.Log("Pre ClamP: " + currentDrillDmg);
                currentDrillDmg = Mathf.Clamp(currentDrillDmg, maxDrillDmg / 4f, maxDrillDmg);
                //Debug.Log("After ClamP: " + currentDrillDmg);
                break;

            }

            case DamageType.rotary:
            {

                currentRotSpeed -= amount;
                currentRotSpeed = Mathf.Clamp(currentRotSpeed, maxRotSpeed / 4f, maxRotSpeed);
                break;
            }
        }
    }
    #endregion
}
public enum DamageType
{
    hull, // damage
    engine, //reduce speed
    drill, // reduce drill damage
    rotary // reduce rotation speed


}
