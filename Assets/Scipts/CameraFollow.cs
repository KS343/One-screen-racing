using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing;

    private void Start()
    {
        transform.position = target.position;
    }

    private void FixedUpdate()
    {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * smoothing);
    }
}
