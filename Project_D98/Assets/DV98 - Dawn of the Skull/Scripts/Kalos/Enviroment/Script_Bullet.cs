using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Bullet : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float speed;

    [SerializeField] float timeToDeactivate;


    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }

    }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        // Inicia la corutina para desactivar el objeto después del tiempo
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        // Espera el tiempo antes de desactivar el objeto
        yield return new WaitForSeconds(timeToDeactivate);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        // Detener la corutina si el objeto se desactiva antes del tiempo
        StopAllCoroutines();
    }


}
