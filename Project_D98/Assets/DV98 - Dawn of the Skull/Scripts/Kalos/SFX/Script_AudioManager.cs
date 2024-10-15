using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_AudioManager : MonoBehaviour
{
    #region SingleTon
    private static Script_AudioManager instance;
    public static Script_AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("No hay AudioManager!");
            }
            return instance;
        }
    }

    #endregion

    //Declaracion de todos los valores de la base de datos (Todos los valores han de ser PUBLIC)

    //Llamada sin referencia: Script_AudioManager.Instance.PlaySFX(1);

    [Header("=== Audio Source References ===")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("=== Clip Library ===")]
    public AudioClip[] musicArray;
    public AudioClip[] sfxArray;


    private void Awake()
    {
        //El Singleton se referencia a si mismo
        instance = this;
    }

    //En un Singleton podemos declarar cualquier ACCION LLAMABLE siempre y cuando sea PUBLIC
    #region Music Methods
    public void PlayMusic(int musicToPlay)
    {
        musicSource.clip = musicArray[musicToPlay];
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    #endregion

    #region SFX Methods

    public void PlaySFX(int sfxToPlay)
    {
        sfxSource.PlayOneShot(sfxArray[sfxToPlay]);
    }

    #endregion
}
