using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class Spaceship_Control : MonoBehaviour
{


    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;
    [SerializeField, Range(0.111f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.111f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
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

    public float currentBoostAmount;

    float glide, verticalGlide, horizontalGlide = 0f;

    Rigidbody rb;


    //Input Values
    private float thrust1D;
    private float strafe1D;
    private float UpDown1D;
    private float roll1D;
    private float pitch1D;
    private float yaw1D;
    private float stabilize;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
        
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

    }


    void FixedUpdate()
    {

        //Stabilize
        HandleStabilize();
        //Boost
        HandleBoosting();
        //Move
        HandleMovement();
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
    void HandleStabilize()
    {
        //Stabilize
        if (stabilize > 0)
        {
            rb.angularDrag = 1.5f;
        }
        else
        {
            rb.angularDrag = 0.5f;
        }
    }

    void HandleBoosting() 
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if (currentBoostAmount<=0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoostAmount <  maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }
    void HandleMovement()
    {
        //Roll
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
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

        //UP / Down
        if (UpDown1D > 0.1f || UpDown1D < -0.1f)
        {
            float currenThrust = thrust;

            rb.AddRelativeForce(Vector3.up * UpDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= thrustGlideReduction;
        }

        //Strafing
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            float currenThrust = thrust;

            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= thrustGlideReduction;
        }

    }



    void HandleShoot()
    {

        if (pumPuming && currentBoostAmount > 0f)
        {
            currentBoostAmount -= boostDeprecationRate;
            if (currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    #region InputsMethods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }
    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }
    public void OnUpDown(InputAction.CallbackContext context)
    {
        UpDown1D = context.ReadValue<float>();
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
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
