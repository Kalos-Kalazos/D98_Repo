using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]

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

    [Header("=== Shoot Settings ===")]
    [SerializeField]
    private float ammo;
    [SerializeField]
    public int damage;
    [SerializeField]
    private float fireCooldown;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float maxAmmo;
    [SerializeField]
    private float pumPuming;
    [SerializeField]
    public float locking;
    [SerializeField]
    private Transform shootingPoint;

    [Header("=== Visual Settings ===")]
    [SerializeField]
    VisualEffect vfx_Boost;

    [Header("=== Inputs Settings ===")]
    [SerializeField]
    private InputAction onMove, onRotate, onStabilize, onShoot, onBoost, onLockIn, onStart, onQuit;


    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void BoostChanged(float currentBoost);
    public event BoostChanged OnBoostChanged;

    public delegate void HeatChanged(float currentHeat);
    public event HeatChanged OnHeatChanged;

    public int vfxDuration;

    float glide, horizontalGlide = 0f;

    Rigidbody rb;


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
        ammo = maxAmmo;

        vfxDuration = Shader.PropertyToID("Duration");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Vfx desde una pool aparece con la colision de una bala

        if (collision.collider.CompareTag("Bullet"))
        {
            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = collision.collider.transform.position;
                explosion.transform.rotation = collision.collider.transform.rotation;
                explosion.SetActive(true);
            }

            TakeDamage(2);
        }

        if (collision.collider.CompareTag("BB"))
        {

            GameObject explosionB = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosionB != null)
            {
                explosionB.transform.SetPositionAndRotation(collision.collider.transform.position, collision.collider.transform.rotation);
                explosionB.SetActive(true);
            }

            TakeDamage(10);
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


        if (fireCooldown <= 0 && pumPuming>0 && !overHeated)
        {
            Shoot();
            fireCooldown = fireRate;
        }
        else
        {
            currentHeat -= Time.deltaTime;

            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }

        if (OnBoostChanged != null)
        {
            OnBoostChanged(currentBoost);
        }

        if (OnHeatChanged != null)
        {
            OnHeatChanged(currentHeat);
        }

        if (currentHeat >= maxHeat)
        {
            overHeated = true;
        }

        if (currentHeat <= 0)
        {
            overHeated = false;
        }

        #region Mantener Health, fireCooldown y Heat en sus limites

        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHeat > maxHeat) currentHeat = maxHeat;

        if (currentHealth < 0) currentHealth = 0;
        if (currentHeat < 0) currentHeat = 0;
        if (fireCooldown < 0) fireCooldown = 0;

        #endregion

    }


    void FixedUpdate()
    {        
        Stabilize();
        
        Boosting();
        
        Movement();
    }

    void Shoot()
    {
        if (ammo > 0)
        {
            ammo--;

            currentHeat++;

            GameObject bullet = Script_ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = shootingPoint.position;
                bullet.transform.rotation = shootingPoint.rotation;
                bullet.SetActive(true);
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
            vfx_Boost.SetFloat(vfxDuration, currentBoost);
        }
        else
        {
            vfx_Boost.SetFloat(vfxDuration, 1f);

            if (currentBoost <  maxBoostAmount)
            {
                currentBoost += boostRechargeRate;
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
