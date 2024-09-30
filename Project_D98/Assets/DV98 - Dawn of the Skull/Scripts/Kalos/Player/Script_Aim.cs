using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class Script_Aim : MonoBehaviour
{
    [Header("=== Aim Settings ===")]
    [SerializeField]
    public Transform currentTarget;
    [SerializeField]
    float lockOnRange = 10f;
    [SerializeField]
    float maxLockDistance = 15f;
    [SerializeField]
    LayerMask enemyLayer;
    [SerializeField]
    float rotationSpeed = 5f;
    [SerializeField]
    public bool locking;
    [SerializeField]
    float angleToTarget;
    [SerializeField]
    float maxLockAngle = 60f;
    [SerializeField]
    private Script_Spaceship control;
    [SerializeField]
    float closestEnemy;
    [SerializeField]
    Collider[] enemiesInRange;

    Quaternion forwardLook;


    void Start()
    {

    }


    void Update()
    {
        forwardLook = control.transform.rotation;

        if (control.locking>0) 
        {
            LockOnTarget();
        }
        else
        {
           if(!locking)
           {
                transform.rotation = Quaternion.Slerp(transform.rotation, forwardLook, Time.deltaTime * rotationSpeed);
                Debug.Log("Resuming locking off");
           }
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
        locking = true;

        // Busca el enemigo más cercano dentro del rango
        enemiesInRange = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 80), lockOnRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            currentTarget = GetClosestEnemy(enemiesInRange).transform;
        }
        else
        {
            currentTarget = null; // Si no hay enemigos cercanos, el objetivo se resetea
            locking = false;
        }
    }

    Transform GetClosestEnemy(Collider[] enemiesInRange)
    {
        closestEnemy = 200;
        Transform bestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            // Calcular el ángulo entre la z dela nave y el enemigo + tiene que estar dentro de el limite
            float angleToEnemy = Vector3.Angle(control.transform.forward, directionToEnemy);

            if (distance < closestEnemy && angleToEnemy < maxLockAngle)
            {
                closestEnemy = distance;
                bestEnemy = enemy.transform;
            }
        }

        Debug.Log("Enemigo seleccionado");
        return bestEnemy;
    }

    void AimAtTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget < maxLockAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
