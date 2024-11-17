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
    public bool startSpawn;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    public float spawnCount=0;
    [SerializeField]
    private float maxSpawnCount;
    [SerializeField]
    public bool puSpawned;

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


        if (spawnCooldown <= 0)
        {
            if (CompareTag("SpawnEnemy") && startSpawn) SpawnEnemy();
            if (!puSpawned && !CompareTag("SpawnEnemy")) SpawnPowerUp();

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
            if (spawnCooldown > 0 && !puSpawned)
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
            if (SceneManager.Equals(SceneManager.GetActiveScene(), SceneManager.GetSceneByName("Scene_LevelBoss")))
            {
                enemy.GetComponent<Script_Enemy>().enabled = false;
                StartCoroutine(ActivateMinionAfterDelay(5, enemy));
            }
            enemy.transform.position = gameObject.transform.position;
            enemy.transform.rotation = gameObject.transform.rotation;
            enemy.SetActive(true);
        }
    }

    IEnumerator ActivateMinionAfterDelay(float delay, GameObject enemy)
    {
        yield return new WaitForSeconds(delay);
        ActiveMinion(enemy);
    }

    void ActiveMinion(GameObject enemy)
    {
        enemy.GetComponent<Script_Enemy>().enabled = true;
    }

    public void SpawnPowerUp()
    {
        GameObject powerUp = Script_ObjectPooling.SharedInstance.GetPooledPU();
        if (powerUp != null)
        {
            powerUp.transform.position = gameObject.transform.position;
            powerUp.transform.rotation = gameObject.transform.rotation;
            powerUp.transform.SetParent(gameObject.transform);
            powerUp.SetActive(true);
            puSpawned = true;
        }
    }

    public void DisableSpawn()
    {
        puSpawned = false;
    }
}
