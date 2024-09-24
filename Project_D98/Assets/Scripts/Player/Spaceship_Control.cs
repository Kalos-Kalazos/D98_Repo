using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class Spaceship_Control : MonoBehaviour
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
    private bool boosting = false;

    [Header("=== Shoot Settings ===")]
    [SerializeField]
    private float ammo;
    [SerializeField]
    private float fireCooldown;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private float maxAmmo;
    [SerializeField]
    private bool pumPuming = false;
    [SerializeField]
    public bool locking = false;
    [SerializeField]
    private Transform shootingPoint;


    public delegate void HealthChanged(int currentHealth);
    public event HealthChanged OnHealthChanged;

    public delegate void BoostChanged(float currentBoost);
    public event BoostChanged OnBoostChanged;


    float glide, horizontalGlide = 0f;

    Rigidbody rb;


    //Input Values
    private float thrust1D;
    private float strafe1D;
    private float pitch1D;
    private float yaw1D;
    private float stabilize;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoost = maxBoostAmount;

        currentHealth = maxHealth;
        ammo = maxAmmo;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Vfx desde una pool aparece con la colision de una bala

        if (collision.collider.CompareTag("Bullet"))
        {
            GameObject explosion = ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = collision.collider.transform.position;
                explosion.transform.rotation = collision.collider.transform.rotation;
                explosion.SetActive(true);
            }

            TakeDamage(2);
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
        if (fireCooldown <= 0 && pumPuming)
        {
            pumPuming = false;
            Shoot();
            fireCooldown = fireRate;
        }
        else
        {
            if (fireCooldown > 0)
            {
                fireCooldown -= Time.deltaTime;
            }
        }

        if (OnBoostChanged != null)
        {
            OnBoostChanged(currentBoost);
        }

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
            ammo -= 1;
            GameObject bullet = ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = shootingPoint.position;
                bullet.transform.rotation = shootingPoint.rotation;
                bullet.SetActive(true);
            }
        }
        else
        {
            fireCooldown = 5;
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
        if (boosting && currentBoost > 0f)
        {
            currentBoost -= boostDeprecationRate;

            if (currentBoost<=0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoost <  maxBoostAmount)
            {
                currentBoost += boostRechargeRate;
            }
        }
    }

    void Movement()
    {
        //Pitch
        rb.AddRelativeTorque(Vector3.up * yaw1D * pitchTorque * Time.deltaTime);
        //Yaw
        rb.AddRelativeTorque(Vector3.right * -pitch1D * yawTorque * Time.deltaTime);

        //Thrust
        if(thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currenThrust;

            if (boosting)
            {
                currenThrust = thrust * boostMultiplier;
            }
            else
            {
                currenThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currenThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        //Strafing
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {

            rb.AddRelativeForce(Vector3.right * strafe1D * strafeThrust * Time.fixedDeltaTime);
            horizontalGlide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }

    }

    //En esta region estan los metodos llamados en el input system y se vinculan con las variables

    #region InputsMethods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnPitch(InputAction.CallbackContext context)
    {
        pitch1D = context.ReadValue<float>();
    }
    public void OnYaw(InputAction.CallbackContext context)
    {
        yaw1D = context.ReadValue<float>();
    }
    public void Stabilize(InputAction.CallbackContext context)
    {
        stabilize = context.ReadValue<float>();
    }
    public void Boost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }
    public void PumPum(InputAction.CallbackContext context)
    {
        pumPuming = context.performed;
    }
    public void LockIn(InputAction.CallbackContext context)
    {
        locking = context.performed;
    }
    #endregion
}
