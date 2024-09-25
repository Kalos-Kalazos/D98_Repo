using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim_Control : MonoBehaviour
{
    public Transform currentTarget;
    public float lockOnRange = 10f;
    public float maxLockDistance = 15f;
    public LayerMask enemyLayer;
    public float rotationSpeed = 5f;

    [SerializeField]
    private Spaceship_Control control;


    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (control.locking) 
        {
            LockOnTarget();
        }

        if (currentTarget != null)
        {
            AimAtTarget(); 

            // Si el objetivo se aleja demasiado, recalcular
            if (Vector3.Distance(transform.position, currentTarget.position) > maxLockDistance)
            {
                LockOnTarget(); 
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
