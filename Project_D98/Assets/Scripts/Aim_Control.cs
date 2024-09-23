using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim_Control : MonoBehaviour
{
    public Transform currentTarget; // El enemigo fijado
    public float lockOnRange = 10f; // Rango para fijar objetivo
    public float maxLockDistance = 15f; // Máxima distancia permitida antes de recalcular objetivo
    public LayerMask enemyLayer; // Capa donde están los enemigos
    public float rotationSpeed = 5f; // Velocidad de rotación del jugador al apuntar

    [SerializeField]
    private Spaceship_Control control;

    // Start is called before the first frame update
    void Start()
    {
    }


    private void AimAtPlayer()
    {
        GameObject enemy = ObjectPooling.SharedInstance.GetPooledEnemy();
        if (enemy != null)
        {
            //enemy.transform.position = collision.collider.transform.position;
            // enemy.transform.rotation = collision.collider.transform.rotation;
            enemy.SetActive(true);
        }

        if (enemy != null)
        {
            Vector3 directionToPlayer = enemy.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(directionToPlayer);

            Debug.DrawLine(transform.position, enemy.transform.position, Color.green);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (control.locking) // Suponiendo que el jugador presiona "L" para hacer lock-in
        {
            LockOnTarget();
        }

        if (currentTarget != null)
        {
            AimAtTarget(); // El jugador apunta al objetivo actual

            // Si el objetivo se aleja demasiado, recalcular
            if (Vector3.Distance(transform.position, currentTarget.position) > maxLockDistance)
            {
                LockOnTarget(); // Recalcula otro objetivo
            }
        }


    }


    void LockOnTarget()
    {
        // Busca el enemigo más cercano dentro del rango
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);
        if (enemiesInRange.Length > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider enemy in enemiesInRange)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                Debug.DrawLine(transform.position, enemy.transform.position, Color.green);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = enemy.transform;
                }
            }



            if (closestEnemy != null)
            {
                currentTarget = closestEnemy;
            }
        }
        else
        {
            currentTarget = null; // Si no hay enemigos cercanos, el objetivo se resetea
        }
    }
    void AimAtTarget()
    {
        // Rotar hacia el objetivo actual
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
