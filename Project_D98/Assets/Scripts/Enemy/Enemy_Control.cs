using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Control : MonoBehaviour
{
    [Header("=== Enemy Ship Movement Settings ===")]
    [SerializeField]
    private float moveSpeed = 10; 
    [SerializeField]
    private float rotationSpeed = 5f; 
    [SerializeField]
    private float angularDrag = 0.1f;
    [SerializeField]
    private float linearDrag = 0.1f; 
    [SerializeField]
    private float ammo;
    [SerializeField]
    private float maxAmmo = 2;
    [SerializeField]
    private bool shooting;
    [SerializeField]
    private float fireCooldown = 0;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float pivotCooldown = 0;

    public Transform player, shootingPoint;

    public GameObject bulletPrefab;
    public Transform playerPivot;

    Rigidbody rb; 

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        player = GameObject.FindWithTag("Player").transform;

        playerPivot = player.GetChild(Random.Range(2, 5));

        ammo = maxAmmo;

        rb.angularDrag = angularDrag;
        rb.drag = linearDrag;
    }

    private void Update()
    {
        //Cooldowns para los disparos y el cambio de pivote de direccion

        if (fireCooldown <= 0)
        {
            ShootPlayer();
            fireCooldown = fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }

        if (pivotCooldown <= 0)
        {
            ResetObjective();
            pivotCooldown = 10;
        }
        else
        {
            pivotCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        PursuePlayer();

        AimAtPlayer();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
            GameObject explosion = ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = collision.collider.transform.position;
                explosion.transform.rotation = collision.collider.transform.rotation;
                explosion.SetActive(true);
            }
        }

    }

    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli

        if (player != null && shootingPoint != null)
        {
            Vector3 directionToPlayer = player.position - shootingPoint.position;

            shootingPoint.rotation = Quaternion.LookRotation(directionToPlayer);

            Debug.DrawLine(shootingPoint.position, player.position, Color.red);
        }
    }

    private void ShootPlayer()
    {
        //Si aun hay municion resto uno, cojo un objeto de la pool y este orientado al shootingPoint

        if (ammo > 0)
        {
            ammo -= 1; 
            GameObject bullet = ObjectPooling.SharedInstance.GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = shootingPoint.transform.position;
                bullet.transform.rotation = shootingPoint.transform.rotation;
                bullet.SetActive(true);
            }
        }
        else
        {
            fireCooldown = 5;
        }
    }

    private void ResetObjective()
    {
        //Cambio el pivote objetivo de la direccion
        playerPivot = player.GetChild(Random.Range(2, 5));

    } 



    private void PursuePlayer()
    {
        //Calculo la direccion de los pivotes del jugador y añado una fuerza hacia el

        if (player != null)
        {
            
            Vector3 directionToPlayer = (playerPivot.position - transform.position).normalized;

            
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            
            rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}
