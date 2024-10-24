using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Spawn_enemy : MonoBehaviour
{
    [Header("=== Spawn Settings ===")]
    [SerializeField]
    private float spawnCooldown;
    [SerializeField]
    private float maxCooldown;
    [SerializeField]
    private bool overLoad = false;
    [SerializeField]
    public bool startSpawn = false;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    public float spawnCount=0;
    [SerializeField]
    private float maxSpawnCount;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnCooldown <= 0) spawnCooldown = 0;

        if (spawnCount > maxSpawnCount)
        {
            spawnCooldown = maxCooldown;
            spawnCount = 0;
        }

        if (spawnCooldown <= 0 && startSpawn)
        {
            if (CompareTag("SpawnEnemy")) SpawnEnemy();
            else SpawnPowerUp();

            if (overLoad)
            {
                spawnCooldown = 0.2f;
            }
            else
            {
                spawnCooldown = spawnRate;
            }
        }
        else
        {
            if (spawnCooldown > 0)
            {
                spawnCooldown -= Time.deltaTime;
            }
        }
    }

    public void SpawnEnemy()
    {
        spawnCount++;

        GameObject enemy = Script_ObjectPooling.SharedInstance.GetPooledEnemy();
        if (enemy != null)
        {
            enemy.transform.position = gameObject.transform.position;
            enemy.transform.rotation = gameObject.transform.rotation;
            enemy.SetActive(true);
        }
    }
    public void SpawnPowerUp()
    {
        GameObject powerUp = Script_ObjectPooling.SharedInstance.GetPooledPU();
        if (powerUp != null)
        {
            powerUp.transform.position = gameObject.transform.position;
            powerUp.transform.rotation = gameObject.transform.rotation;
            powerUp.SetActive(true);
        }
    }
}
