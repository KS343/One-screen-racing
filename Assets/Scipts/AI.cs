using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform path;
    private List<Transform> nodes;
    public int currentNode = 0;

    private float maxCarSpeed;
    public float maxAIallowedSpeed;
    public float currentSpeed;
    private float steerAngle;
    public float turnSpeed;

    public float startSensorLength = 1f;
    [SerializeField]
    private float sensorLength;
    public Vector3 frontSensorPos = new Vector3 (0, 0.1f, 0.15f);
    public float frontSideSensorPos = 0.3f;
    public float FrontEdgeSensorAngle = 30f;

    private bool avoiding = false;
    public float avoidMultiplier;
    public int obstacleLayerMask;
    public int outOfTrackLayerMask;
    
    public bool isBreaking;

    public float roadAngleMultiplier = 100;

    public bool OutOfTrack = false;
    private void Start()
    {
        maxCarSpeed = gameObject.GetComponent<Car>().motorTorque;
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        obstacleLayerMask = LayerMask.NameToLayer("AvoidObstacle");
        outOfTrackLayerMask = LayerMask.NameToLayer("OutOfTrack");

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
                nodes.Add(pathTransforms[i]);
        }
    }

    private void Update()
    {
        ApplySteer();
        Debug.Log(currentNode);
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

    public float Drive(float wheelRadius, float wheelRPM)
    {
        float throttle;
        //SetSpeedOnRoadTurn();
        currentSpeed = -1 * 2 * Mathf.PI * wheelRadius * wheelRPM * 60 / 1000;
        if(currentSpeed <maxAIallowedSpeed)
        {
            throttle = 1;
        }
        else
        {
            throttle = 0;
        }
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
        if(Vector3.Distance(transform.position,nodes[currentNode].position) < 0.5f)
        {
            CountRoadAngle();
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

    private void CountRoadAngle()
    {
        if(currentNode == 0)
        {
            Vector3 vector1direction = nodes[nodes.Count-1].position - nodes[currentNode].position;
            Vector3 vector2direction = nodes[currentNode + 1].position - nodes[currentNode].position;
            roadAngleMultiplier = (int)Vector3.Angle(vector1direction, vector2direction);
        }
        else if(currentNode == nodes.Count-1)
        {
            Vector3 vector1direction = nodes[nodes.Count - 2].position - nodes[currentNode].position;
            Vector3 vector2direction = nodes[0].position - nodes[currentNode].position;
            roadAngleMultiplier = (int)Vector3.Angle(vector1direction, vector2direction);
        }
        else 
        {
            Vector3 vector1direction = nodes[currentNode - 1].position - nodes[currentNode].position;
            Vector3 vector2direction = nodes[currentNode + 1].position - nodes[currentNode].position;
            roadAngleMultiplier = (int)Vector3.Angle(vector1direction, vector2direction);
        }
        if (roadAngleMultiplier > 90)
            roadAngleMultiplier %= 90;
        roadAngleMultiplier = (roadAngleMultiplier/90) * 100;
        if (roadAngleMultiplier >= 0 && roadAngleMultiplier < 10)
            roadAngleMultiplier = 10;
        if (roadAngleMultiplier < 90 && roadAngleMultiplier > 75)
        {
            roadAngleMultiplier = 100;
        }
        Debug.Log("RoadAngle is " + roadAngleMultiplier);
    }

    public void SetSpeedOnRoadTurn()
    {
        if (roadAngleMultiplier == 100)
            {
            return;
            }
        else
        {
            if (currentSpeed / maxCarSpeed * 100 > roadAngleMultiplier)
            {
                isBreaking = true;
            }
            else
            {
                isBreaking = false;
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

        if (OutOfTrack)
        {
            sensorLength /= 3;
        }
        else sensorLength = startSensorLength;

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
        if(OutOfTrack)
        {
            wheelSteerAngle *= 3;
        }
        return wheelSteerAngle;
    }
}