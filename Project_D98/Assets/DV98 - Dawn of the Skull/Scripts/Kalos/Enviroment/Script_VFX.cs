using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_VFX : MonoBehaviour
{

    [SerializeField] float timeToDeactivate = 3f;

    void OnEnable()
    {
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
