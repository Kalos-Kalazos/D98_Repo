using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_ObjectPooling : MonoBehaviour
{

    public static Script_ObjectPooling SharedInstance;

    [Header("=== Pool Settings ===")]
    //Bullets
    [SerializeField]
    List<GameObject> pooledBullets;
    [SerializeField]
    GameObject bulletToPool;
    [SerializeField]
    int amountToPoolB;

    //Explosions
    [SerializeField]
    List<GameObject> pooledExplosions;
    [SerializeField]
    GameObject explosionToPool;
    [SerializeField]
    int amountToPoolE;

    //Enemies
    [SerializeField]
    List<GameObject> pooledEnemy;
    [SerializeField]
    GameObject enemyToPool;
    [SerializeField]
    int amountToPoolX;

    //Big Explosions "BE"
    [SerializeField]
    List<GameObject> pooledBE;
    [SerializeField]
    GameObject beToPool;
    [SerializeField]
    int amountToPoolBE;

    //Big Bullets "BB"
    [SerializeField]
    List<GameObject> pooledBB;
    [SerializeField]
    GameObject bbToPool;
    [SerializeField]
    int amountToPoolBB;

    //Power Ups
    [SerializeField]
    List<GameObject> pooledPU;
    [SerializeField]
    GameObject puToPool;
    [SerializeField]
    int amountToPoolPU;

    //Power Up Missiles
    [SerializeField]
    List<GameObject> pooledMissile;
    [SerializeField]
    GameObject missileToPool;
    [SerializeField]
    int amountToPoolMissile;

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

        pooledBB = new List<GameObject>();
        GameObject tmpBB;
        for (int i = 0; i < amountToPoolBB; i++)
        {
            tmpBB = Instantiate(bbToPool);
            tmpBB.SetActive(false);
            pooledBB.Add(tmpBB);
        }

        pooledPU = new List<GameObject>();
        GameObject tmpPU;
        for (int i = 0; i < amountToPoolPU; i++)
        {
            tmpPU = Instantiate(puToPool);
            tmpPU.SetActive(false);
            pooledPU.Add(tmpPU);
        }

        pooledMissile = new List<GameObject>();
        GameObject tmpMS;
        for (int i = 0; i < amountToPoolMissile; i++)
        {
            tmpMS = Instantiate(missileToPool);
            tmpMS.SetActive(false);
            pooledMissile.Add(tmpMS);
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
    public GameObject GetPooledBB()
    {
        for (int i = 0; i < amountToPoolBB; i++)
        {
            if (!pooledBB[i].activeInHierarchy)
            {
                return pooledBB[i];
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
    public GameObject GetPooledFS()
    {
        for (int i = 0; i < amountToPoolPU; i++)
        {
            if (!pooledPU[i].activeInHierarchy)
            {
                return pooledPU[i];
            }
        }
        return null;
    }
    public GameObject GetPooledMissile()
    {
        for (int i = 0; i < amountToPoolMissile; i++)
        {
            if (!pooledMissile[i].activeInHierarchy)
            {
                return pooledMissile[i];
            }
        }
        return null;
    }

}
