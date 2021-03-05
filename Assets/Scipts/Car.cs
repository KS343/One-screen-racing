using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float motorTorque;
    public float maxSteer;
    public float breakingTorque;
    public float currentBreakingTorque;

    public float Steer { get; set; }
    public float Throttle { get; set; }

    private Rigidbody rb;
    public Wheel[] wheels;
    public Vector3 centerOfMass1;


    private void Start()
    {
        wheels = GetComponentsInChildren<Wheel>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMass1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.01f);
    }

    public void Update()
    {
        foreach (var wheel in wheels)
        {
            wheel.steerAngle = Steer * maxSteer;
            wheel.torque = Throttle * motorTorque;
            wheel.breakTorque = currentBreakingTorque;
        }
    }
}