using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_LaserEyes : MonoBehaviour
{
    public Transform player, pivotPoint;
    public GameObject bulletPrefab;
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
    private bool shooting, empty, cantShoot, locked;
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
    private RaycastHit rayCastHit;
    [SerializeField]
    GameObject laserVFX;
    [SerializeField]
    GameObject flashVFX;



    // Start is called before the first frame update
    void Start()
    {
        padreControl = padre.GetComponent<Script_Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0 && !dead && !cantShoot)
        {
            ShootPlayer();
            fireCooldown = fireRate;
            laserVFX.SetActive(true);
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
        if (health > 0)
        {
            AimAtPlayer();
        }
    }

    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli con velocidad configurable

        if (player != null && !locked)
        {
            if (charge <= 0)
            {
                flashVFX.SetActive(true);
            }
            else
            {
                charge -= Time.deltaTime;
            }

            Vector3 directionToPlayer = player.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            angleToTarget = Vector3.Angle(padre.transform.forward, directionToPlayer);

            if (angleToTarget < maxLockAngle)
            {
                //Roto el punto de disparo dentro de un rango
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedTurret);
                cantShoot = false;
            }
            else cantShoot = true;


            Debug.DrawLine(transform.position, player.position, Color.cyan);
        }
    }
    private void ShootPlayer()
    {
        //Si aun hay municion resto uno, cojo un objeto de la pool y este orientado al shootingPoint

        if (!dead && locked)
        {
            
        }
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
