using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateTimerDisplay : MonoBehaviour
{
    Slider timerSlider;
    float currentTime;

    private void Start()
    {
        timerSlider = gameObject.GetComponent<Slider>();
        timerSlider.maxValue = WaveManager.instance.timeBetweenWaveSpawns;
    }
    void Update()
    {
        currentTime = WaveManager.instance.waveTimer.timeCurrent;
        timerSlider.value = currentTime;
    }
} 
