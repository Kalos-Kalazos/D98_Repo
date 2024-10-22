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

    [Header("=== Sprite Settings ===")]
    [SerializeField]
    GameObject topLeftSprite;
    [SerializeField]
    GameObject topRightSprite;
    [SerializeField]
    GameObject bottomLeftSprite;
    [SerializeField]
    GameObject bottomRightSprite;
    [SerializeField]
    float spriteMoveSpeed = 2;
    [SerializeField]
    bool stayOnTarget;
    [SerializeField]
    float spriteScale;

    Vector3 initialOffsetTL, initialOffsetTR, initialOffsetBR, initialOffsetBL;

    Quaternion forwardLook;

    Transform cameraTransform;

    void Start()
    {
        if (control == null)
        {
            control = GetComponentInParent<Script_Spaceship>();
        }

        #region Sprite initial ofssets
        initialOffsetTL = new Vector3(-8, 8, 0);
        initialOffsetTR = new Vector3(8, 8, 0);
        initialOffsetBR = new Vector3(-8, -8, 0);
        initialOffsetBL = new Vector3(8, -8, 0);
        #endregion

        SetSpritesActive(false);

        stayOnTarget = false;

        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
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

            if(!stayOnTarget) SpriteInitialPositions(currentTarget.position);
            else SpritePositions(currentTarget.position);

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
        SetSpritesActive(false);
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
                if (currentTarget != closestEnemy)
                {
                    stayOnTarget = false;
                }
                currentTarget = closestEnemy;
                SetSpritesActive(true);
            }
            else
            {
                currentTarget = null; 
                locking = false;
                SetSpritesActive(false);
                stayOnTarget = false;
            }
        }
        else
        {
            currentTarget = null; // Si no hay enemigos cercanos, el objetivo se resetea
            locking = false;
            SetSpritesActive(false);
            stayOnTarget = false;
        }
    }

    Transform GetClosestEnemy(Collider[] enemiesInRange)
    {
        closeEnemy = 10000;
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

    void SetSpritesActive(bool active)
    {
        topLeftSprite.SetActive(active);
        topRightSprite.SetActive(active);
        bottomLeftSprite.SetActive(active);
        bottomRightSprite.SetActive(active);
    }
    void SpriteInitialPositions(Vector3 targetPos)
    {
        if (topLeftSprite == null || topRightSprite == null || bottomLeftSprite == null || bottomRightSprite == null) return;

        Vector3 halfwayPoint = (targetPos + control.transform.position) / 2;
        Vector3 direction = (targetPos - control.transform.position).normalized;

        topLeftSprite.transform.position = halfwayPoint + initialOffsetTL + Quaternion.Euler(0, 45, 0) * direction * 2;
        topRightSprite.transform.position = halfwayPoint + initialOffsetTR + Quaternion.Euler(0, -45, 0) * direction * 2;
        bottomLeftSprite.transform.position = halfwayPoint + initialOffsetBL + Quaternion.Euler(0, 135, 0) * direction * 2;
        bottomRightSprite.transform.position = halfwayPoint + initialOffsetBR + Quaternion.Euler(0, -135, 0) * direction * 2;

        stayOnTarget = true;
    }

    void SpritePositions(Vector3 targetPos)
    {
        if (currentTarget == null || topLeftSprite == null || topRightSprite == null || bottomLeftSprite == null || bottomRightSprite == null) return;

        float step = spriteMoveSpeed * Time.deltaTime;

        Vector3 halfwayPoint = (targetPos + control.transform.position) / 2;

        Vector3 direction = (targetPos - control.transform.position).normalized;

        topLeftSprite.transform.position = Vector3.Lerp(topLeftSprite.transform.position, halfwayPoint + Quaternion.Euler(0, 45, 0) * direction * 2, step);
        topRightSprite.transform.position = Vector3.Lerp(topRightSprite.transform.position, halfwayPoint + Quaternion.Euler(0, -45, 0) * direction * 2, step);
        bottomLeftSprite.transform.position = Vector3.Lerp(bottomLeftSprite.transform.position, halfwayPoint + Quaternion.Euler(0, 135, 0) * direction * 2, step);
        bottomRightSprite.transform.position = Vector3.Lerp(bottomRightSprite.transform.position, halfwayPoint + Quaternion.Euler(0, -135, 0) * direction * 2, step);

        topLeftSprite.transform.LookAt(cameraTransform);
        topRightSprite.transform.LookAt(cameraTransform);
        bottomLeftSprite.transform.LookAt(cameraTransform);
        bottomRightSprite.transform.LookAt(cameraTransform);
    }

}
