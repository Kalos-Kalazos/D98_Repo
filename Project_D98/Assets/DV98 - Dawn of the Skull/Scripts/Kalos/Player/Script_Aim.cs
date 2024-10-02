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
    float lockOnRange;
    [SerializeField]
    float maxLockDistance;
    [SerializeField]
    LayerMask enemyLayer;
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    public bool locking;
    [SerializeField]
    float angleToTarget;
    [SerializeField]
    float maxLockAngle;
    [SerializeField]
    private Script_Spaceship control;
    [SerializeField]
    float closeEnemy;
    [SerializeField]
    Collider[] enemiesInRange;
    [SerializeField]
    float lockCount;
    [SerializeField]
    bool isLocked;

    Quaternion forwardLook;


    void Start()
    {
        if (control == null)
        {
            control = GetComponentInParent<Script_Spaceship>();
        }
    }


    void Update()
    {

        forwardLook = control.transform.rotation;

        if (control.locking>0)
        {
            if (!isLocked)
            {
                LockOnTarget();
                isLocked = true;
            }
        }
        else
        {
            if (isLocked)
            {
                ReleaseLock();
                isLocked = false;
            }
        }

        if (!locking)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, forwardLook, Time.deltaTime * rotationSpeed);
            Debug.Log("Resuming locking off");
            for (int i = 0; i < enemiesInRange.Length; i++)
            {
                enemiesInRange[i] = null;
            }
        }

        if (currentTarget != null && isLocked)
        {
            AimAtTarget();

            // Si el objetivo se aleja demasiado, recalcular
            if (Vector3.Distance(transform.position, currentTarget.position) > maxLockDistance)
            {
                LockOnTarget();
            }
        }
    }
    void ReleaseLock()
    {
        currentTarget = null;
        locking = false;
    }

    void LockOnTarget()
    {
        if (control == null) return;

        locking = true;

        // Busca el enemigo más cercano dentro del rango
        enemiesInRange = Physics.OverlapSphere(transform.position, lockOnRange, enemyLayer);

        if (enemiesInRange.Length > 0)
        {
            Transform closestEnemy = GetClosestEnemy(enemiesInRange); 
            if (closestEnemy != null)
            {
                currentTarget = closestEnemy; 
            }
            else
            {
                currentTarget = null; 
                locking = false;
            }
        }
        else
        {
            currentTarget = null; // Si no hay enemigos cercanos, el objetivo se resetea
            locking = false;
        }
    }

    Transform GetClosestEnemy(Collider[] enemiesInRange)
    {
        closeEnemy = 200;
        Transform bestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            // Calcular el ángulo entre la z dela nave y el enemigo + tiene que estar dentro de el limite
            float angleToEnemy = Vector3.Angle(control.transform.forward, directionToEnemy);

            if (distance < closeEnemy && angleToEnemy < maxLockAngle)
            {
                closeEnemy = distance;
                bestEnemy = enemy.transform;
            }
        }

        Debug.Log("Enemigo seleccionado");
        return bestEnemy;
    }

    void AimAtTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        angleToTarget = Vector3.Angle(transform.forward, direction);

        if (angleToTarget < maxLockAngle)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
