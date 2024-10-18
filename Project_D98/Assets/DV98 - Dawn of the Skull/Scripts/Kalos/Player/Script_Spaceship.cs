using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class Script_Spaceship : MonoBehaviour
{
    [Header("=== Status Settings ===")]
    [SerializeField]
    public int currentHealth;
    [SerializeField]
    public int maxHealth;
    [SerializeField]
    public float currentBoost;
    [SerializeField]
    public float maxBoost;
    [SerializeField]
    public float currentHeat;
    [SerializeField]
    public float maxHeat;
    [SerializeField]
    public bool overHeated = false;

    [Header("=== Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float strafeThrust = 50f;
    [SerializeField, Range(0.111f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.111f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    [SerializeField]
    bool returning;

    [Header("=== Boost Settings ===")]
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechargeRate = 0.5f;
    [SerializeField]
    private float boostMultiplier = 5f;
    [SerializeField]
    private float boosting;
    [SerializeField]
    private bool resetBoost;
    [SerializeField]
    private bool isBoostActive;
    [SerializeField]
    private bool isVFXBoost;

    [Header("=== Shoot Settings ===")]
    [SerializeField]
    public int damage;
    [SerializeField]
    private float fireCooldown;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float pumPuming;
    [SerializeField]
    public float locking;
    [SerializeField]
    private Transform shootingPoint;

    [Header("=== Power Up Settings ===")]
    // Fast shooting
    [SerializeField]
    public bool fastShooting;
    [SerializeField]
    public float fsCooldown;
    [SerializeField]
    float fsRate;
    // Double Shooting
    [SerializeField]
    public bool doubleShooting;
    [SerializeField]
    public float dsCooldown;
    [SerializeField]
    float dsRate;
    [SerializeField]
    public int shootsNum;
    // Area Shooting
    [SerializeField]
    public bool areaShooting;
    [SerializeField]
    public float asCooldown;
    [SerializeField]
    float asRate;


    [Header("=== Visual Settings ===")]
    [SerializeField]
    VisualEffect[] vfx_Boost;

    [Header("=== Inputs Settings ===")]
    [SerializeField]
    private InputAction onMove, onRotate, onStabilize, onShoot, onBoost, onLockIn, onStart, onQuit;


    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void BoostChanged(float currentBoost);
    public event BoostChanged OnBoostChanged;

    public delegate void HeatChanged(float currentHeat);
    public event HeatChanged OnHeatChanged;

    float glide, horizontalGlide = 0f;

    Rigidbody rb;

    [SerializeField]
    GameObject limitArea;
    [SerializeField]
    CinemachineVirtualCamera cameraPlayer;

    //Input Values
    public Vector2 moveValue;
    private Vector2 rotateValue;
    private float stabilize;

    void Start()
    {
        #region Inputs Enabled

        onMove.Enable();
        onRotate.Enable();
        onStabilize.Enable();
        onShoot.Enable();
        onBoost.Enable();
        onLockIn.Enable();
        onStart.Enable();
        onQuit.Enable();

        #endregion

        rb = GetComponent<Rigidbody>();
        currentBoost = maxBoostAmount;

        currentHealth = maxHealth;

        for (int i = 0; i < vfx_Boost.Length; i++)
        {
            vfx_Boost[i].Stop();
        }

        returning = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        //Vfx desde una pool aparece con la colision de una bala

        if (other.CompareTag("Bullet") || other.CompareTag("BB"))
        {
            if (other.CompareTag("Bullet")) TakeDamage(2);
            
            else TakeDamage(10);

        }

        if (other.CompareTag("Limit"))
        {
            Invoke(nameof(ResetReturning), 1);
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (OnHealthChanged != null)
        {
            OnHealthChanged(currentHealth);
        }
    }

    private void Update()
    {

        #region Read Inputs

        moveValue = onMove.ReadValue<Vector2>();
        rotateValue = onRotate.ReadValue<Vector2>();
        stabilize = onStabilize.ReadValue<float>();
        boosting = onBoost.ReadValue<float>();
        pumPuming = onShoot.ReadValue<float>();
        locking = onLockIn.ReadValue<float>();

        #endregion

        #region Shooting logic
        if (fireCooldown <= 0 && pumPuming>0 && !overHeated)
        {
            if (fastShooting && fsCooldown > 0)
            {
                if (doubleShooting && dsCooldown > 0)
                {
                    if (areaShooting && asCooldown > 0)
                    {
                        AreaShoot();
                        fireCooldown = fsRate;
                    }
                    else
                    {
                        DoubleShoot();
                        fireCooldown = fsRate;
                    }
                }
                else
                {
                    if (areaShooting && asCooldown > 0)
                    {
                        AreaShoot();
                        fireCooldown = fsRate;
                    }
                    else
                    {
                        Shoot();
                        fireCooldown = fsRate;
                    }
                }
            }
            else
            {
                if (doubleShooting && dsCooldown > 0)
                {
                    DoubleShoot();
                    fireCooldown = fireRate;
                }
                else
                {
                    if (areaShooting && asCooldown > 0)
                    {
                        AreaShoot();
                        fireCooldown = asRate;
                    }
                    else
                    {
                        Shoot();
                        fireCooldown = fireRate;
                    }
                }
            }

        }
        else
        {
            currentHeat -= Time.deltaTime;

            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }
        #endregion

        #region Cooldown power ups
        if (fsCooldown > 0)
        {
            fsCooldown -= Time.deltaTime;
        }else fastShooting = false;

        if (dsCooldown > 0)
        {
            dsCooldown -= Time.deltaTime;
        }
        else
        {
            shootsNum = 1;
            doubleShooting = false;
        }

        if (asCooldown > 0)
        {
            asCooldown -= Time.deltaTime;
        }
        else
        {
            damage = 1;
            areaShooting = false;
        }
        #endregion

        #region boost and heat bar updates
        if (OnBoostChanged != null)
        {
            OnBoostChanged(currentBoost);
        }

        if (OnHeatChanged != null)
        {
            OnHeatChanged(currentHeat);
        }
        #endregion

        #region Mantener Health, fireCooldown y Heat en sus limites

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHeat > maxHeat) currentHeat = maxHeat;

        if (currentHealth < 0) currentHealth = 0;
        if (currentHeat < 0) currentHeat = 0;
        if (fireCooldown < 0) fireCooldown = 0;

        if (currentHeat >= maxHeat)
        {
            overHeated = true;
        }
        if (currentHeat <= 0)
        {
            overHeated = false;
        }

        #endregion

    }

    void FixedUpdate()
    {        
        Stabilize();
        
        Boosting();

        if (returning) ReturnToArea();
        else Movement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Limit"))
        {
            returning = true;
        }
    }

    void ResetReturning()
    {
        returning = false;
    }

    void ReturnToArea()
    {
        Vector3 directionToCenter = (transform.position - limitArea.transform.position).normalized;


        rb.AddRelativeTorque(directionToCenter * yawTorque * Time.deltaTime);
        rb.AddRelativeTorque(directionToCenter * pitchTorque * Time.deltaTime);

        rb.AddForce(transform.forward * 20 * Time.deltaTime, ForceMode.VelocityChange);
    }

    void Shoot()
    {
        if (!fastShooting) currentHeat++;
        else currentHeat += 0.25f;

            GameObject bullet = Script_ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.GetComponent<Script_Bullet>().damageBullet = damage;
                bullet.transform.position = shootingPoint.position;
                bullet.transform.rotation = shootingPoint.rotation;
                Script_AudioManager.Instance.PlaySFX(0);
                bullet.SetActive(true);
            }

    }

    void DoubleShoot()
    {
        if (!fastShooting)
            currentHeat++;
        else
            currentHeat += 0.25f;

        for (int i = 0; i < shootsNum; i++)
        {
            GameObject bullet = Script_ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.GetComponent<Script_Bullet>().damageBullet = damage;
                //                       if                        else
                Vector3 offset = (i == 0) ? new Vector3(i-1, 0, 0) : new Vector3(i+1, 0, 0);
                bullet.transform.position = shootingPoint.position + offset;
                bullet.transform.rotation = shootingPoint.rotation;
                Script_AudioManager.Instance.PlaySFX(0);
                Invoke(nameof(SoundDelayed), 0.1f);
                bullet.SetActive(true);
            }
        }
    }

    void SoundDelayed()
    {
        Script_AudioManager.Instance.PlaySFX(0);
    }


    void AreaShoot()
    {
        if (!fastShooting)
            currentHeat+=2;
        else
            currentHeat++;

        for (int i = 0; i < shootsNum; i++)
        {
            GameObject missile = Script_ObjectPooling.SharedInstance.GetPooledMissile();
            if (missile != null)
            {
                missile.GetComponent<Script_Missile>().damageMissile = damage;
                Vector3 offset = (i == 0) ? new Vector3(i + 1, 0, 0) : new Vector3(i - 1, 0, 0);
                missile.transform.position = shootingPoint.position; //+ offset;
                missile.transform.rotation = shootingPoint.rotation;
                missile.SetActive(true);
            }
        }
    }

    void Stabilize()
    {
        if (stabilize > 0)
        {
            rb.angularDrag = 1.5f;
        }
        else
        {
            rb.angularDrag = 0.5f;
        }
    }

    void Boosting() 
    {
        if (boosting>0 && currentBoost > 0f && !resetBoost)
        {
            currentBoost -= boostDeprecationRate;

            if (!isBoostActive)
            {
                isBoostActive = true;

                if (!isVFXBoost)
                {
                    for (int i = 0; i < vfx_Boost.Length; i++)
                    {
                        vfx_Boost[i].Play();
                    }

                    isVFXBoost = true;
                    cameraPlayer.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 3;
                }
            }
        }
        else
        {

            if (currentBoost <  maxBoostAmount)
            {
                currentBoost += boostRechargeRate;
            }

            if (isBoostActive)
            {
                isBoostActive = false;

                if (isVFXBoost)
                {
                    for (int i = 0; i < vfx_Boost.Length; i++)
                    {
                        vfx_Boost[i].Stop();
                    }

                    isVFXBoost = false;
                    cameraPlayer.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0.5f;
                }
            }
        }
    }

    void Movement()
    {
        //Pitch
        rb.AddRelativeTorque(Vector3.up * rotateValue.x * pitchTorque * Time.deltaTime);
        //Yaw
        rb.AddRelativeTorque(Vector3.right * -rotateValue.y * yawTorque * Time.deltaTime);

        //Thrust
        if(moveValue.y > 0.1f || moveValue.y < -0.1f)
        {
            float currenThrust;

            if (boosting>0)
            {
                currenThrust = thrust * boostMultiplier;
            }
            else
            {
                currenThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * moveValue.y * currenThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        //Strafing
        if (moveValue.x > 0.1f || moveValue.x < -0.1f)
        {

            rb.AddRelativeForce(Vector3.right * moveValue.x * strafeThrust * Time.fixedDeltaTime);
            horizontalGlide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }

    }

}
