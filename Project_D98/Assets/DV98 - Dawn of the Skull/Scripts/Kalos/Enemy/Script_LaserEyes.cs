using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_LaserEyes : MonoBehaviour
{
    public Transform player;
    public Transform padre;

    Script_Boss padreControl;

    Script_GameManager gameManager;

    Rigidbody rb;

    public bool hit, dead;

    Vector3 explosionPos;

    [Header("=== Turret Combat Settings ===")]
    [SerializeField]
    public float health;
    [SerializeField]
    private bool shooting, empty, cantShoot, locked, reseted;
    [SerializeField]
    private float fireCooldown;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float rotationSpeedTurret;
    [SerializeField]
    private float angleToTarget;
    [SerializeField]
    private float maxLockAngle = 60;
    [SerializeField]
    private float charge;
    [SerializeField]
    private float distanceRay;
    [SerializeField]
    private float radiusRay;
    [SerializeField]
    private RaycastHit rayCastHit;
    [SerializeField]
    private float damageLaser;
    [SerializeField]
    GameObject laserVFX;
    [SerializeField]
    GameObject flashVFX;

    RaycastHit rayHit;

    Vector3 directionToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        padreControl = padre.GetComponent<Script_Boss>();
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        
        laserVFX.SetActive(false);
        flashVFX.SetActive(false);
        reseted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0 && !dead && !cantShoot)
        {
            if (CompareTag("LaserBall") && padreControl.health < 2)
            {
                ShootPlayer();
            }
            else if (!CompareTag("LaserBall")) ShootPlayer();
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }

        if (health <= 0 && !dead)
        {
            rb.constraints = RigidbodyConstraints.None;
            player.GetComponentInChildren<Script_Aim>().locking = false;

            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledBE();
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.transform.rotation = transform.rotation;
                explosion.SetActive(true);
            }

            if (player != null)
            {
                if (player.GetComponentInChildren<Script_Aim>().currentTarget == gameObject.transform)
                {
                    player.GetComponentInChildren<Script_Aim>().currentTarget = null;
                    player.GetComponentInChildren<Script_Aim>().locking = false;
                }
            }

            if (padreControl != null)
            {
                padreControl.health--;
            }

            dead = true;
        }
    }

    private void FixedUpdate()
    {
        if (health > 0 && fireCooldown <= 0)
        {
            if (CompareTag("LaserBall") && padreControl.health < 2) AimAtPlayer(); 
            else if(!CompareTag("LaserBall")) AimAtPlayer();
        }

    }

    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli con velocidad configurable

        if (player != null)
        {
            directionToPlayer = player.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            angleToTarget = Vector3.Angle(padre.transform.forward, directionToPlayer);

            if (angleToTarget < maxLockAngle)
            {
                //Roto el punto de disparo dentro de un rango
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedTurret);

                if (charge <= 0)
                {
                    cantShoot = false;
                }
                else
                {
                    charge -= Time.deltaTime;
                    cantShoot = true;
                    flashVFX.SetActive(true);
                }
            }

           // Debug.DrawLine(transform.position, player.position, Color.cyan);
        }
    }
    private void ShootPlayer()
    {
        if (!dead && !cantShoot)
        {
            Vector3 direction = transform.forward;
            if (Physics.SphereCast(transform.position, radiusRay, direction, out rayHit, distanceRay))
            {
                if (rayHit.rigidbody != null) rayHit.rigidbody.AddForce(direction.normalized, ForceMode.Impulse); 

                laserVFX.SetActive(true);
                flashVFX.SetActive(true);

                if(!reseted) Invoke(nameof(ResetShoot), 7);

                Script_Spaceship targetShip = rayHit.collider.gameObject.GetComponent<Script_Spaceship>();
                if (targetShip != null)
                {
                    targetShip.TakeDamage(damageLaser);
                }
                
            }

        }
    }

    void ResetShoot()
    {
        charge = 20;
        fireCooldown = fireRate;
        reseted = true;
        Invoke(nameof(ResetReset),1);
    }

    void ResetReset()
    {
        reseted = false;
    }

    public void Hitted(Collider other)
    {
        explosionPos = other.transform.position;
        hit = true;
        health -= player.gameObject.GetComponent<Script_Spaceship>().damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Hitted(collision.collider);
        }
    }
}
