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
    int powerID;
    [SerializeField]
    int count;
    [SerializeField]
    Material[] puMaterial;

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
            GetComponent<MeshRenderer>().material = puMaterial[powerID + 1];

            switch (powerID)
            {
                case 1:
                    player.fastShooting = true;
                    player.fsCooldown = 30;
                    gameObject.SetActive(false);
                break;

                case 2:
                    player.doubleShooting = true;
                    player.dsCooldown = 30;
                    player.shootsNum++;
                    gameObject.SetActive(false);
                break;

                case 3:
                    player.areaShooting = true;
                    player.damage = 6;
                    player.asCooldown = 40;
                    gameObject.SetActive(false);
                break;

                case 4:
                    player.Heal(50);
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
