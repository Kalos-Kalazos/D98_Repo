using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Script_PowerUp : MonoBehaviour
{

    private Script_Spaceship player;

    private Script_Spawn_enemy spawn;

    [Header("=== Power Up Settings ===")]
    [SerializeField]
    float powerID;
    [SerializeField]
    int count;

    void Start()
    {
        player = FindObjectOfType<Script_Spaceship>();
    }

    private void OnEnable()
    {
        powerID = Random.Range(1, 5);

        spawn = GetComponentInParent<Script_Spawn_enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (powerID)
            {
                case 1:
                    player.fastShooting = true;
                    player.fsCooldown = 15;
                    gameObject.SetActive(false);
                break;

                case 2:
                    player.doubleShooting = true;
                    player.dsCooldown = 15;
                    player.shootsNum++;
                    gameObject.SetActive(false);
                break;

                case 3:
                    player.areaShooting = true;
                    player.damage = 4;
                    player.asCooldown = 25;
                    gameObject.SetActive(false);
                break;

                case 4:
                    player.Heal(25);
                    gameObject.SetActive(false);
                break;
            }
        }
    }

    private void OnDisable()
    {
        if(spawn != null)
        {
            spawn.DisableSpawn();
        }
    }
}
