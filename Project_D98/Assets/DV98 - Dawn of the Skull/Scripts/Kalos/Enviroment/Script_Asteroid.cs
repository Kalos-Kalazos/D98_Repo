using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Asteroid : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Bullet") || other.CompareTag("BB"))
        {
            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.transform.rotation = transform.rotation;
                explosion.SetActive(true);
                other.gameObject.SetActive(false);
                Destroy(gameObject);
            }

        }
    }
}
