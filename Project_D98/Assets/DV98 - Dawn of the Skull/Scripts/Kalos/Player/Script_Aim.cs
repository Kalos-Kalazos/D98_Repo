using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class Script_Aim : MonoBehaviour
{
    public Transform currentTarget;
    public float lockOnRange = 10f;
    public float maxLockDistance = 15f;
    public LayerMask enemyLayer;
    public float rotationSpeed = 5f;
    public bool locking;

    Quaternion forwardLook;

    public float closestEnemy;

    public Collider[] enemiesInRange;

    [SerializeField]
    private Script_Spaceship control;

    CapsuleCollider capsuleAim;

    void Start()
    {
        capsuleAim = GetComponent<CapsuleCollider>();
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

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawSphere(transform.forward + new Vector3(0, 0, 80), lockOnRange);

    }


    void LockOnTarget()
    {

        locking = true;
        Debug.Log("Locking true");


        // Busca el enemigo más cercano dentro del rango
        enemiesInRange = Physics.OverlapSphere(transform.localPosition, lockOnRange, enemyLayer);


        //enemiesInRange = Physics.OverlapCapsule(transform.position+new Vector3(0,0,40), transform.position+new Vector3(0, 0, 150), lockOnRange, enemyLayer);
        

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

    Collider GetClosestEnemy(Collider[] enemiesInRange)
    {
        closestEnemy = 200;
        Collider bestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestEnemy)
            {
                closestEnemy = distance;
                bestEnemy = enemy;
            }
        }
        Debug.Log("Enemigo seleccionado");
        return bestEnemy;
    }

    void AimAtTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
