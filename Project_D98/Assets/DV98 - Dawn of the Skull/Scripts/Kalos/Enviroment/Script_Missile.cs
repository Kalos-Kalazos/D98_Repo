using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Missile : MonoBehaviour
{
    Rigidbody rb;


    [Header ("=== Missile Settings ===")]
    [SerializeField] float speed;
    [SerializeField] float timeToDeactivate;
    [SerializeField] float power;
    [SerializeField] float radius;
    [SerializeField] public float damageMissile;
    [SerializeField] Collider[] entitiesPushed;


    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(timeToDeactivate);

        Explode();
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        entitiesPushed = Physics.OverlapSphere(transform.position, radius);

        GameObject missileVFX = Script_ObjectPooling.SharedInstance.GetPooledMissileVFX();
        if (missileVFX != null)
        {
            missileVFX.transform.position = transform.position;
            missileVFX.transform.rotation = transform.rotation;
            missileVFX.SetActive(true);
        }


        if (entitiesPushed.Length > 0)
        {
            for (int i = 0; i < entitiesPushed.Length; i++)
            {
                if (entitiesPushed[i].TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(power, transform.position, radius, 3);
                }
                if (entitiesPushed[i].gameObject.CompareTag("Enemy"))
                {
                    if (entitiesPushed[i].gameObject.GetComponent<Script_Enemy>() != null)
                    {
                        entitiesPushed[i].TryGetComponent(out Script_Enemy enemy);
                        enemy.Hitted(entitiesPushed[i]);
                    }

                    if (entitiesPushed[i].gameObject.GetComponent<Script_Turret>() != null)
                    {
                        entitiesPushed[i].TryGetComponent(out Script_Turret enemyTurret);
                        enemyTurret.Hitted(entitiesPushed[i]);
                    }

                }
            }
        }

        gameObject.SetActive(false);

    }
}
