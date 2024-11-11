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
    [SerializeField]
    GameObject[] UI_Image;
    void Start()
    {
        player = FindObjectOfType<Script_Spaceship>();
    }

    private void OnEnable()
    {
        powerID = Random.Range(0, 4);

        spawn = GetComponentInParent<Script_Spawn_enemy>();

        GetComponent<MeshRenderer>().material = puMaterial[powerID];


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Script_AudioManager.Instance.PlaySFX(4);

            switch (powerID)
            {
                case 0:
                    player.fastShooting = true;
                    player.fsCooldown = 30;
                    gameObject.SetActive(false);
                    UI_Image[powerID].SetActive(true);

                    break;

                case 1:
                    player.doubleShooting = true;
                    player.dsCooldown = 30;
                    player.shootsNum++;
                    gameObject.SetActive(false);
                    UI_Image[powerID].SetActive(true);
                    break;

                case 2:
                    player.areaShooting = true;
                    player.damage = 6;
                    player.asCooldown = 40;
                    gameObject.SetActive(false);
                    UI_Image[powerID].SetActive(true);
                    break;

                case 3:
                    player.Heal(50);
                    gameObject.SetActive(false);
                    UI_Image[powerID].SetActive(true);
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
