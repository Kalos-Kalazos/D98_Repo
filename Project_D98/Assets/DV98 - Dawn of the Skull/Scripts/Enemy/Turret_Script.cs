using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Script : MonoBehaviour
{
    public Transform player, shootPointFront, pivot;
    public GameObject bulletPrefab;
    public Boss_Control padre;

    [Header("=== Turret Combat Settings ===")]
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
    private float rotationSpeedTurret = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        padre = FindObjectOfType<Boss_Control>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fireCooldown <= 0)
        {
            ShootPlayer();
            fireCooldown = fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        AimAtPlayer();
    }

    private void AimAtPlayer()
    {
        //Calculo la direccion del jugador y hago el shootingPoint mirar hacia alli con velocidad configurable

        if (player != null && shootPointFront != null)
        {
            Vector3 directionToPlayer = player.position - shootPointFront.position;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            //Roto el punto de disparo
            shootPointFront.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedTurret);


            Debug.DrawLine(shootPointFront.position, player.position, Color.cyan);
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
                bullet.transform.position = pivot.transform.position;
                bullet.transform.rotation = pivot.transform.rotation;
                bullet.SetActive(true);
            }
        }
        else
        {
            fireCooldown = 5;
        }
    }
}
