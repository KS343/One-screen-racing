using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool steer;
    public bool invertSteer;
    public bool power;

    public float steerAngle { get; set; }
    public float torque { get; set; }
    public float breakTorque;
    public WheelCollider wheelCollider;
    private Transform wheelTransform;
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void FixedUpdate()
    {
        if(steer)
        {
            wheelCollider.steerAngle = steerAngle * (invertSteer ? -1 : 1);
        }

        if(power)
        {  
                wheelCollider.motorTorque = -torque;
                wheelCollider.brakeTorque = breakTorque;
        }
    }
}
