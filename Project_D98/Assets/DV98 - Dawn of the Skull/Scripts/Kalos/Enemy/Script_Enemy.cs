using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Enemy : MonoBehaviour
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
    private bool shooting, empty;
    [SerializeField]
    private float fireCooldown = 0;
    [SerializeField]
    private float fireRate = 1;
    [SerializeField]
    private float pivotCooldown = 0;


   public Script_GameManager gameManager;

    public Transform shootingPoint;

    public GameObject player;

    public GameObject bulletPrefab;
    public Transform playerPivot;

    Rigidbody rb; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindWithTag("Player");

        gameManager = FindObjectOfType<Script_GameManager>();

        playerPivot = player.transform.GetChild(Random.Range(2, 5));

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
        if (collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("BB"))
        {
            gameObject.SetActive(false);
            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = collision.collider.transform.position;
                explosion.transform.rotation = collision.collider.transform.rotation;
                explosion.SetActive(true);
            }

            Achieved();
        }
    }
    void Achieved()
    {
        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Tutorial")))
        {
            gameManager.LvlCompleted();
        }

        if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_Level1")) && gameManager.spawner.spawnCount >= 5)
        {
            gameManager.LvlCompleted();
        }
    }
    private void OnDisable()
    {
        if (player != null)
        {
            if (player.GetComponentInChildren<Script_Aim>().currentTarget == gameObject.transform)
            {
                player.GetComponentInChildren<Script_Aim>().currentTarget = null;
                player.GetComponentInChildren<Script_Aim>().locking = false;
            }
        }

    }
    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli

        if (player != null && shootingPoint != null)
        {
            Vector3 directionToPlayer = player.transform.position - shootingPoint.position;

            shootingPoint.rotation = Quaternion.LookRotation(directionToPlayer);

            Debug.DrawLine(shootingPoint.position, player.transform.position, Color.red);
        }
    }
    private void ShootPlayer()
    {
        //Si aun hay municion resto uno, cojo un objeto de la pool y este orientado al shootingPoint

        if (ammo > 0 && !empty)
        {
            ammo -= 1; 
            GameObject bullet = Script_ObjectPooling.SharedInstance.GetPooledBullet();
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
        playerPivot = player.transform.GetChild(Random.Range(2, 5));

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
