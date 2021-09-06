using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    public float xMax;
    public float xMin;
    public float zMax;
    public float zMin;

    private void Start()
    {
        transform.position = target.position;
    }

    private void FixedUpdate()
    {

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothing);

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.z = Mathf.Clamp(pos.z, zMin, zMax);

        transform.position = pos;
    }
}
