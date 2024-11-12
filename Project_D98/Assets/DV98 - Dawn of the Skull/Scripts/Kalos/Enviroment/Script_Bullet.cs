using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Bullet : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float speed;

    [SerializeField] float timeToDeactivate;

    [SerializeField] public float damageBullet;

    public string parentTag;

    private void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            if (parentTag != collision.gameObject.tag)
            {
                GameObject hitted = Script_ObjectPooling.SharedInstance.GetPooledHitVFX();
                if (hitted != null)
                {
                    ContactPoint contact = collision.GetContact(0);
                    hitted.transform.SetPositionAndRotation(contact.point, Quaternion.FromToRotation(Vector3.up, contact.normal));
                    SetParentDecal(collision, hitted);
                    hitted.SetActive(true);
                }
            }

        }
        if (!collision.collider.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);            
        }
    }

    void SetParentDecal(Collision collision, GameObject hitted)
    {
        hitted.transform.SetParent(collision.transform);
    }
    void ResetParentDecal(GameObject hitted)
    {
        hitted.transform.SetParent(null);
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
