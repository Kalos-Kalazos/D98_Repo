using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Asteroid : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") || collision.collider.CompareTag("BB"))
        {
            GameObject explosion = Script_ObjectPooling.SharedInstance.GetPooledExplosion();
            if (explosion != null)
            {
                explosion.transform.position = transform.position;
                explosion.transform.rotation = transform.rotation;
                explosion.SetActive(true);
                Destroy(gameObject);
            }

        }
    }
}
