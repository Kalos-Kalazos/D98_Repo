using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//LEER - SIEMPRE QUE SE AÑADA ESTE SCRIPT TIENE QUE ESTAR EL COMPONENTE "AUDIO SOURCE"
public class Script_Audio : MonoBehaviour
{

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();

            //Destroy(gameObject); Para los Asteroides
        }
    }

}
