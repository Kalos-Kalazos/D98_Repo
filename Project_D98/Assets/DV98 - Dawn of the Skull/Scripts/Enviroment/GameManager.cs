using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Slider healthSlider;           
    public Slider boostSlider;             
    private Spaceship_Control player;         
    

    void Start()
    {
        player = FindObjectOfType<Spaceship_Control>();

        if (player != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.currentHealth;
            boostSlider.maxValue = player.maxBoost;
            boostSlider.value = player.currentBoost;

            player.OnHealthChanged += UpdateHealthBar;
            player.OnBoostChanged += UpdateBoostBar;
        }
    }

    #region Cambios de Slider de Vida, Boost y Heat

    public void UpdateHealthBar(int currentHealth)
    {
        StartCoroutine(HealthChange(currentHealth));
    }
    IEnumerator HealthChange(int targetHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.5f; 
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetHealth, elapsedTime / duration);
            yield return null;
        }

        healthSlider.value = targetHealth;
    }

    public void UpdateBoostBar(float currentBoost)
    {
        StartCoroutine(BoostChange(currentBoost));
    }
    IEnumerator BoostChange(float targetBoost)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startValue = boostSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            boostSlider.value = Mathf.Lerp(startValue, targetBoost, elapsedTime / duration);
            yield return null;
        }

        boostSlider.value = targetBoost;
    }

    //Falta el de Heat

    #endregion
}
