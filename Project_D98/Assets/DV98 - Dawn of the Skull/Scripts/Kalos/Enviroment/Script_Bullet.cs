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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player") || collision.collider.gameObject.CompareTag("Enemy") || collision.collider.gameObject.CompareTag("Boss"))
        {
            GameObject hitted = Script_ObjectPooling.SharedInstance.GetPooledHitVFX();
            if (hitted != null)
            {
                ContactPoint contact = collision.GetContact(0);
                hitted.transform.SetPositionAndRotation(contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
                hitted.SetActive(true);
            }

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
