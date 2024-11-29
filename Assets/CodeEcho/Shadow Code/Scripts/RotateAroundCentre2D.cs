using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCentre2D : MonoBehaviour
{
    public float speed = 1.0f;

    void Update()
    {
        transform.localRotation *= Quaternion.Euler(0, 0, speed * Time.deltaTime);
    }
}
