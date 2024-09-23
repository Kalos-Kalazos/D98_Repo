using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    BoxCollider col;

    [Header("General parameters")]
    [SerializeField] GameObject deathEffect;
    [SerializeField] GameObject body;

    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            body.SetActive(false);
            deathEffect.SetActive(true);
            col.enabled = false;
        }
    }
}
