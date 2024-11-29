using System;
using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;

public class DecoderCharge : MonoBehaviour
{
    public float chargeSpeed = 1f;
    public float drainSpeed = 2f;

    private float charge = 0;
    private bool charging = true;
    private bool draining = false;

    private Shape chargeBar;

    void Start()
    {
        chargeBar = GetComponent<Shape>();
        EventManager.StartListening("DecoderEnabled", this);
        EventManager.StartListening("DecoderDisabled", this);
    }

    void Update()
    {
        if (charging)
        {
            if (charge == 100) return;
            charge = Mathf.Min(100, charge + (chargeSpeed * Time.deltaTime));
            if (charge == 100)
            {
                EventManager.Trigger("DecoderCharged");
            }
        }
        if (draining)
        {
            if (charge == 0) return;
            charge = Mathf.Max(0, charge - (drainSpeed * Time.deltaTime));
            if (charge == 0)
            {
                EventManager.Trigger("DecoderDrained");
            }
        }
        UpdateChargeBar();
    }

    private void UpdateChargeBar()
    {
        var arc = 181 + (179 * (charge / 100));
        chargeBar.settings.endAngle = arc;
    }

    void OnDecoderEnabled()
    {
        charging = false;
        draining = true;
    }

    void OnDecoderDisabled()
    {
        charging = true;
        draining = false;
    }
}
