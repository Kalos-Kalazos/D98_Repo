using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_PowerUp : MonoBehaviour
{

    private Script_Spaceship player;

    [Header("=== Power Up Settings ===")]
    [SerializeField]
    float powerID;

    void Start()
    {
        player = FindObjectOfType<Script_Spaceship>();
    }

    private void OnEnable()
    {
        powerID = Random.Range(1, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (powerID)
            {
                case 1:
                    player.fastShooting = true;
                    player.fsCooldown = 20;
                    gameObject.SetActive(false);
                break;

                case 2:
                    player.doubleShooting = true;
                    player.dsCooldown = 20;
                    gameObject.SetActive(false);
                break;
            }
        }
    }

    void Update()
    {
        
    }
}
