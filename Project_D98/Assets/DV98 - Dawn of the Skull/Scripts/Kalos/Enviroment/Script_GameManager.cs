using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_GameManager : MonoBehaviour
{
    public Slider healthSlider;           
    public Slider boostSlider;
    public Slider heatSlider;
    private Script_Spaceship player;         
    
    public void Restart()
    {
        SceneManager.LoadScene("Kalos_Scene");
    }

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        player = FindObjectOfType<Script_Spaceship>();

        if (player != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.currentHealth;

            boostSlider.maxValue = player.maxBoost;
            boostSlider.value = player.currentBoost;

            heatSlider.maxValue = player.maxHeat;
            heatSlider.value = player.currentHeat;

            player.OnHealthChanged += UpdateHealthBar;
            player.OnBoostChanged += UpdateBoostBar;
            player.OnHeatChanged += UpdateHeatBar;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.currentHealth <= 0)
            {
                SceneManager.LoadScene("YouDied");
            }
        }
    }

    #region Cambios de Slider de Vida, Boost y Heat

    //Health bar update
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

    //Boost bar update
    public void UpdateBoostBar(float currentBoost)
    {
        StartCoroutine(BoostChange(currentBoost));
    }
    IEnumerator BoostChange(float targetBoost)
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        float startValue = boostSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            boostSlider.value = Mathf.Lerp(startValue, targetBoost, elapsedTime / duration);
            yield return null;
        }

        boostSlider.value = targetBoost;
    }

    //Heat bar update
    public void UpdateHeatBar(float currentHeat)
    {
        StartCoroutine(HeatChange(currentHeat));
    }
    IEnumerator HeatChange(float targetHeat)
    {
        float elapsedTime = 0f;
        float duration = 0.1f;
        float startValue = heatSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            heatSlider.value = Mathf.Lerp(startValue, targetHeat, elapsedTime / duration);
            yield return null;
        }

        heatSlider.value = targetHeat;
    }

    #endregion
}
