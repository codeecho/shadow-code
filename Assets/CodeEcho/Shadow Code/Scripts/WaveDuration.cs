using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDuration : MonoBehaviour
{
    public int index = 1;
    public float duration = 10f;

    private WaveController waveController;

    private bool started = false;
    private float elapsed = 0f;

    void Awake()
    {
        waveController = GetComponent<WaveController>();
        EventManager.StartListening("Wave" + index + "Start", this, OnWaveStart);
    }

    void Update()
    {
        if (!started) return;
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            EventManager.Trigger("Wave" + index + "Ended");
            waveController.enabled = false;
            Destroy(this.gameObject);
            EventManager.Trigger("Wave" + (index + 1) + "Start");
        }
    }

    void OnWaveStart()
    {
        started = true;
        waveController.enabled = true;
    }
}
