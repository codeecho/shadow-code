using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRandomPosition : MonoBehaviour
{
    public float minX = -1f;
    public float maxX = 1f;

    public void Move()
    {
        float x = Random.Range(minX, maxX);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
