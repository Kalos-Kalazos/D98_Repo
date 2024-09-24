using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionControl : MonoBehaviour
{

    [SerializeField] float timeToDeactivate = 3f;

    void OnEnable()
    {
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
