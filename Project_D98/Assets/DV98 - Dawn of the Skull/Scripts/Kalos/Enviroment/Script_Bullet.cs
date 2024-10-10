using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Bullet : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float speed;

    [SerializeField] float timeToDeactivate;

    [SerializeField] public float damageBullet;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(timeToDeactivate);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }


}
