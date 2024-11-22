using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [Header("Skyboxes")]
    [SerializeField] Material[] dayMats;
    [SerializeField] Material[] nightMats;
    private Material[,] allMats;
    // Day-> 0
    // Night-> 1
    // Clear, Rainy, Snowy-> 0, 1, 2

    [Header("Sun")]
    [SerializeField] Light sun;
    [SerializeField] float targetIntensity;
    [SerializeField] Color targetColor;
    private float velocity = 0;


    private void Update() {
        // Smooth our suns intensity to target.
        sun.intensity = Mathf.SmoothDamp(sun.intensity, targetIntensity, ref velocity, 2);

        // Smooth our suns color to target.
        sun.color = Color.Lerp(sun.color, targetColor, .01f);
    }

    // Build our allMats array to make choosing a skybox easier.
    private void Start() {
        allMats = new Material[2, dayMats.Length];

        for(int i=0; i<2; i++)
        {
            for(int j=0; j<dayMats.Length; j++)
            {
                if(i == 0)
                {
                    allMats[i, j] = dayMats[j];
                }
                else
                {
                    allMats[i, j] = nightMats[j];
                }
            }
        }
    }

    // Determine which skybox and lighting to display based on weather code.
    public void DetermineEnvironment(int weatherCode, string timeOfDay)
    {
        // Different if it is day or night.
        if(timeOfDay == "Day")
        {
            if(weatherCode == 800)                                      // 800-> Clearing
            {
                RenderSettings.skybox = allMats[0, 0];
                targetIntensity = 2;
                targetColor = Color.white;
            }
            else if(weatherCode >= 200 && weatherCode <= 531)           // 200-532 -> Rain
            {
                RenderSettings.skybox = allMats[0, 1];
                targetIntensity = 1.5f;
                targetColor = Color.grey;
            }
            else                                                        // Snow
            {
                RenderSettings.skybox = allMats[0, 2];
                targetIntensity = 1.6f;
                targetColor = Color.grey;
            }
        }
        else
        {
            if(weatherCode == 800)                                      // 800-> Clearing
            {
                RenderSettings.skybox = allMats[1, 0];
                targetIntensity = 1f;
                targetColor = Color.grey;
            }
            else if(weatherCode >= 200 && weatherCode <= 531)           // 200-532 -> Rain
            {
                RenderSettings.skybox = allMats[1, 1];
                targetIntensity = 0;
                targetColor = Color.black;
            }
            else                                                        // Snow
            {
                RenderSettings.skybox = allMats[1, 2];
                targetIntensity = .2f;
                targetColor = Color.black;
            }
        }
    }
}
