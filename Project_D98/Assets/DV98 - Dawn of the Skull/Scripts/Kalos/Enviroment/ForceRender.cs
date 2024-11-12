using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRender : MonoBehaviour
{
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();

        // Asegúrate de que el objeto esté activado
        gameObject.SetActive(true);

        // Asegúrate de que el Renderer no se desactive
        if (objectRenderer != null)
        {
            objectRenderer.enabled = true;
        }
    }

    void Update()
    {
        // Forzar a que el objeto siempre sea visible
        if (objectRenderer != null)
        {
            objectRenderer.enabled = true;
        }
    }
}
