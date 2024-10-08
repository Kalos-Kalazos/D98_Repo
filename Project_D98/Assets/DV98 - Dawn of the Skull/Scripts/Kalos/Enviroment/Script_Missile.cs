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

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }

    void Explode()
    {
        entitiesPushed = Physics.OverlapSphere(transform.position, radius);

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
