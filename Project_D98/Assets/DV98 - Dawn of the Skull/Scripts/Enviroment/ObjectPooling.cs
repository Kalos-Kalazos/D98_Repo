using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{

    public static ObjectPooling SharedInstance;

    [Header("=== Pool Settings ===")]
    [SerializeField]
    List<GameObject> pooledBullets;
    [SerializeField]
    GameObject bulletToPool;
    [SerializeField]
    int amountToPoolB;
    [SerializeField]
    List<GameObject> pooledExplosions;
    [SerializeField]
    GameObject explosionToPool;
    [SerializeField]
    int amountToPoolE;
    [SerializeField]
    List<GameObject> pooledEnemy;
    [SerializeField]
    GameObject enemyToPool;
    [SerializeField]
    int amountToPoolX;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledBullets = new List<GameObject>();
        GameObject tmpB;
        for (int i = 0; i < amountToPoolB; i++)
        {
            tmpB = Instantiate(bulletToPool);
            tmpB.SetActive(false);
            pooledBullets.Add(tmpB);
        }


        pooledExplosions = new List<GameObject>();
        GameObject tmpE;
        for (int i = 0; i < amountToPoolE; i++)
        {
            tmpE = Instantiate(explosionToPool);
            tmpE.SetActive(false);
            pooledExplosions.Add(tmpE);
        }

        pooledEnemy = new List<GameObject>();
        GameObject tmpX;
        for (int i = 0; i < amountToPoolX; i++)
        {
            tmpX = Instantiate(enemyToPool);
            tmpX.SetActive(false);
            pooledEnemy.Add(tmpX);
        }
    }

    public GameObject GetPooledBullet()
    {
        for (int i = 0; i < amountToPoolB; i++)
        {
            if (!pooledBullets[i].activeInHierarchy)
            {
                return pooledBullets[i];
            }
        }
        return null;
    }
    public GameObject GetPooledExplosion()
    {
        for (int i = 0; i < amountToPoolE; i++)
        {
            if (!pooledExplosions[i].activeInHierarchy)
            {
                return pooledExplosions[i];
            }
        }
        return null;
    }
    public GameObject GetPooledEnemy()
    {
        for (int i = 0; i < amountToPoolX; i++)
        {
            if (!pooledEnemy[i].activeInHierarchy)
            {
                return pooledEnemy[i];
            }
        }
        return null;
    }

}
