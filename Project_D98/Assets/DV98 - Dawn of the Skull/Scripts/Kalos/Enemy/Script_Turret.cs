using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Turret : MonoBehaviour
{
    public Transform player, pivot;
    public GameObject bulletPrefab;
    public Transform padre;

    Script_Boss padreControl;

    Script_GameManager gameManager;

    Rigidbody rb;

    public bool hit, dead;

    Vector3 explosionPos;

    [Header("=== Turret Combat Settings ===")]
    [SerializeField]
    private float health;
    [SerializeField]
    private float ammo;
    [SerializeField]
    private float maxAmmo = 200;
    [SerializeField]
    private bool shooting, empty, cantShoot;
    [SerializeField]
    private float fireCooldown;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float rotationSpeedTurret;
    [SerializeField]
    private float power = 50;
    [SerializeField]
    private float angleToTarget;
    [SerializeField]
    private float maxLockAngle = 60;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        gameManager = FindObjectOfType<Script_GameManager>();

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
        {
            padreControl = FindObjectOfType<Script_Boss>();
        }

        rb = GetComponent<Rigidbody>();

        ammo = maxAmmo;

        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0 && !dead && !cantShoot)
        {
            ShootPlayer();
            fireCooldown = fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }

        if (health <= 0 && !dead)
        {
            rb.constraints = RigidbodyConstraints.None;
            player.GetComponentInChildren<Script_Aim>().locking = false;

            //                  power     pos  radius and modifier
            rb.AddExplosionForce(power, explosionPos, 5,      3);

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

            if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Tutorial")))
            {
                gameManager.StartSpawn();
            }

            if (padreControl!=null)
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

        if (player != null)
        {
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

        if (ammo > 0 && !dead)
        {
            ammo -= 1;
            GameObject bullet = Script_ObjectPooling.SharedInstance.GetPooledBB();
            if (bullet != null)
            {
                bullet.transform.position = pivot.transform.position;
                bullet.transform.rotation = pivot.transform.rotation;
                bullet.SetActive(true);
            }
        }
        else
        {
            fireCooldown = 50;
            ammo = maxAmmo;
        }
    }

    public void Hitted(Collider other)
    {
        explosionPos = other.transform.position;
        hit = true;
        health-=player.gameObject.GetComponent<Script_Spaceship>().damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Hitted(other);
        }

    }
}
