using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Turret : MonoBehaviour
{
    public Transform player, pivot;
    public GameObject bulletPrefab;
    public Script_Boss padre;

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
    private bool shooting, empty;
    [SerializeField]
    private float fireCooldown = 0;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float rotationSpeedTurret = 1;
    [SerializeField]
    private float power = 50;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        padre = FindObjectOfType<Script_Boss>();

        rb = GetComponent<Rigidbody>();

        ammo = maxAmmo;

        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0 && health > 0)
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

            dead = true;
        }

    }

    private void FixedUpdate()
    {
        if (health > 0)
        {
            AimAtPlayer();
        }
        else
        {
            //                  power     pos  radius and modifier
            rb.AddExplosionForce(power, explosionPos, 5, 3);

        }
    }

    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli con velocidad configurable

        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            //Roto el punto de disparo
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedTurret);


            Debug.DrawLine(transform.position, player.position, Color.cyan);
        }
    }

    private void ShootPlayer()
    {
        //Si aun hay municion resto uno, cojo un objeto de la pool y este orientado al shootingPoint

        if (ammo > 0)
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            explosionPos = collision.transform.position;
            hit = true;
            health--;
            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.SetPositionAndRotation(collision.collider.transform.position, collision.collider.transform.rotation);
                explosion.SetActive(true);
            }
        }
    }
}
