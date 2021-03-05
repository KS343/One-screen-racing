using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform path;
    private List<Transform> nodes;
    private int currentNode = 0;

    public float maxSpeed;
    public float currentSpeed;
    private float steerAngle;
    public float turnSpeed;

    public float sensorLength = 1f;
    public Vector3 frontSensorPos = new Vector3 (0, 0.1f, 0.15f);
    public float frontSideSensorPos = 0.3f;
    public float FrontEdgeSensorAngle = 30f;

    private bool avoiding = false;
    public float avoidMultiplier;
    protected int obstacleLayerMask;
    

    public bool isBreaking;
    private void Start()
    {
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        obstacleLayerMask = LayerMask.NameToLayer("AvoidObstacle");

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }

    private void Update()
    {
        ApplySteer();
    }

    public void ApplySteer()
    {
        if (avoiding)
        {
            steerAngle = avoidMultiplier;
        }
        else
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
            float AiSteer = relativeVector.x / relativeVector.magnitude;
            steerAngle = AiSteer;
        }
    }

    public int Drive(float wheelRadius, float wheelRPM)
    {
        int throttle;
        currentSpeed = -1 * 2 * Mathf.PI * wheelRadius * wheelRPM * 60 / 1000;
        if(currentSpeed <maxSpeed)
        {
            throttle = 1;
        }
        else
        {
            throttle = 0;
        }
        //print(currentSpeed);
        return throttle;
    }

    public float Breaking(float breakingTorque)
    {
        if (isBreaking)
        {
            return breakingTorque;
        }
        else
            return 0f;
    }

    public void CheckNodeDistance()
    {
        if(Vector3.Distance(transform.position,nodes[currentNode].position) <1.3f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    public void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += -transform.forward * frontSensorPos.z;
        sensorStartPos += transform.up * frontSensorPos.y;
        avoiding = false;

        //Right Sensor
        sensorStartPos +=transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
        {
            if (hit.collider.gameObject.layer != obstacleLayerMask)
            {

            }
            else
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += -1f;
            }
        }

        //Right angle Sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-FrontEdgeSensorAngle,transform.up) * -transform.forward, out hit, sensorLength))
        {
            if (hit.collider.gameObject.layer != obstacleLayerMask)
            {

            }
            else
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += -0.5f;
            }
        }

        //Left Sensor
        sensorStartPos -= transform.right * frontSideSensorPos * 2;
        if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
        {
            if (hit.collider.gameObject.layer != obstacleLayerMask)
            {

            }
            else
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }

        //Left angle Sensor
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(FrontEdgeSensorAngle, transform.up) * -transform.forward, out hit, sensorLength))
        {
            if (hit.collider.gameObject.layer != obstacleLayerMask)
            {

            }
            else
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }

        //Middle sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, -transform.forward, out hit, sensorLength))
            {
                if (hit.collider.gameObject.layer != obstacleLayerMask)
                    return;
                else
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if (hit.normal.x < 0)
                        avoidMultiplier = -1f;
                    else
                        avoidMultiplier = 1f;
                }
            }
        }

        if (avoidMultiplier > 1f)
            avoidMultiplier = 1f;
        else if (avoidMultiplier < -1f)
            avoidMultiplier = -1f;
        if (!avoiding)
            avoidMultiplier = 0;
    }
    public float LerpToSteerAngle(float wheelSteerAngle)
    {
        wheelSteerAngle = Mathf.Lerp(wheelSteerAngle,steerAngle,Time.deltaTime * turnSpeed);
        return wheelSteerAngle;
    }
}
