using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{

    public static ObjectPooling SharedInstance;

    [Header("=== Pool Settings ===")]
    //Bullets
    [SerializeField]
    List<GameObject> pooledBullets;
    [SerializeField]
    GameObject bulletToPool;
    [SerializeField]
    int amountToPoolB;
    //Explosiones
    [SerializeField]
    List<GameObject> pooledExplosions;
    [SerializeField]
    GameObject explosionToPool;
    [SerializeField]
    int amountToPoolE;
    //Enemigos
    [SerializeField]
    List<GameObject> pooledEnemy;
    [SerializeField]
    GameObject enemyToPool;
    [SerializeField]
    int amountToPoolX;
    //Big Explosiones "BE"
    [SerializeField]
    List<GameObject> pooledBE;
    [SerializeField]
    GameObject beToPool;
    [SerializeField]
    int amountToPoolBE;

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

        pooledBE = new List<GameObject>();
        GameObject tmpXL;
        for (int i = 0; i < amountToPoolBE; i++)
        {
            tmpXL = Instantiate(beToPool);
            tmpXL.SetActive(false);
            pooledBE.Add(tmpXL);
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
    public GameObject GetPooledBE()
    {
        for (int i = 0; i < amountToPoolBE; i++)
        {
            if (!pooledBE[i].activeInHierarchy)
            {
                return pooledBE[i];
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
