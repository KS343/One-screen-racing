using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum ControlType { Human, AI }
    public ControlType controlType = ControlType.AI;

    public PositionManager positionManager;

    public float bestLapTime =Mathf.Infinity;
    public float lastLapTime = 0;
    public float currentLapTime = 0;
    public int currentLap = 0;
    public int carPos = 1;
    public int passedCheckpoints = 0;

    public int lastCheckpoint = 0;

    private Transform checkpointsParent;
    private int checkpointCount;
    private int checkpointLayer;

    private Car currentCar;
    private AI thisAI;

    private List<Transform> checkpointsTransform;
    private Vector3 startPosition;
    private Quaternion startQuaternion;

    // Start is called before the first frame update
    void Awake()
    {
        checkpointsParent = GameObject.FindWithTag("Checkpoint").transform;
        checkpointCount = checkpointsParent.childCount;
        checkpointLayer = LayerMask.NameToLayer("Checkpoint");
        currentCar = gameObject.GetComponent<Car>();

        startPosition = transform.position;
        startQuaternion = transform.rotation;

        positionManager = positionManager = GameObject.FindObjectOfType<PositionManager>();

            thisAI = gameObject.GetComponent<AI>();

        Transform[] tmpCheckpointsTransforms = checkpointsParent.GetComponentsInChildren<Transform>();
        checkpointsTransform = new List<Transform>();

        for (int i = 0; i < checkpointCount; i++)
        {
            if (tmpCheckpointsTransforms[i] != checkpointsParent.transform)
                checkpointsTransform.Add(tmpCheckpointsTransforms[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentLapTime += Time.deltaTime;
        if (controlType == ControlType.Human)
        {
            currentCar.Steer = GameManager.Instance.InputController.SteerInput;
            currentCar.Throttle = GameManager.Instance.InputController.ThrottleInput;
        }
        else if(controlType == ControlType.AI)
        {
            thisAI.Sensors();
            currentCar.Steer = -thisAI.LerpToSteerAngle(currentCar.Steer);
            currentCar.Throttle = thisAI.Drive(currentCar.wheels[3].wheelCollider.radius, currentCar.wheels[3].wheelCollider.rpm);
            currentCar.currentBreakingTorque = thisAI.Breaking(currentCar.breakingTorque);
            thisAI.CheckNodeDistance();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == thisAI.obstacleLayerMask && controlType == ControlType.AI)
        {
            StopCoroutine(IsStuckCheck(currentLap, lastCheckpoint));
            StartCoroutine(IsStuckCheck(currentLap, lastCheckpoint));
            Debug.Log("Coroutine started");
        }
    }

    void StartLap()
    {
        currentLap++;
        passedCheckpoints++;
        lastCheckpoint = 1;
        currentLapTime = 0;
        Debug.Log("Start Lap!");
    }

    void EndLap()
    {
        lastLapTime = currentLapTime;
        bestLapTime = Mathf.Min(lastLapTime, bestLapTime);
        Debug.Log("End Lap! " + bestLapTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == thisAI.outOfTrackLayerMask && controlType == ControlType.AI)
        {
            thisAI.OutOfTrack = true;
        }

        if (collider.gameObject.layer != checkpointLayer)
        {
            return;
        }

        if(collider.gameObject.name == "1")
        {
            if(lastCheckpoint == checkpointCount)
            {
                EndLap();
                //Debug.Log(passedCheckpoints);
            }

            if(currentLap ==0 || lastCheckpoint == checkpointCount)
            {
                StartLap();
                //Debug.Log(passedCheckpoints);
            }
            return;
        }

        if(collider.gameObject.name == (lastCheckpoint+1).ToString())
        {
            Debug.Log("New Checkpoint");
            lastCheckpoint++;
            passedCheckpoints++;
            positionManager.UpdatePlayerPosData();
            Debug.Log(passedCheckpoints);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == thisAI.outOfTrackLayerMask && controlType == ControlType.AI)
        {
            thisAI.OutOfTrack = false;
        }
    }

    public void Respawn(int currentLap, int lastCheckpoint)
    {
        if (currentLap == 0)
        {
            transform.position = startPosition;
            transform.rotation = startQuaternion;
        }
        else
        {
            transform.position = checkpointsTransform[lastCheckpoint].position;
        }
    }

    public IEnumerator IsStuckCheck(int currentLap, int lastCheckpoint)
    {
        Vector3 startTransform = transform.position;
        yield return new WaitForSeconds(3f);
        Debug.Log("5 seconds passed in coroutine");
        if (Mathf.Abs(transform.position.x - startTransform.x) < 0.5f || Mathf.Abs(transform.position.z - startTransform.z) < 0.5f)
        {
            Respawn(currentLap, lastCheckpoint);
        }
    }
}