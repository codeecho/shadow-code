using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public FloatObject powerUpLevel;

    private SwitchableBulletController gun;

    void Start()
    {
        gun = GetComponent<SwitchableBulletController>();
        gun.SetActiveController((int)powerUpLevel.Get() + 1);
        EventManager.StartListening("PowerUpCollected", this, OnPowerUpCollected);
    }

    private void OnPowerUpCollected()
    {
        powerUpLevel.Set(powerUpLevel.Get() + 1);
        gun.SetActiveController((int)powerUpLevel.Get() + 1);
    }
}
