using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PositionManager : MonoBehaviour
{
    public Transform playersParent;
    public Transform checkPointParent;
    private int checkPointCount;
    public Vector3[] checkPointsPosition;
    public int playersCount;
    public PlayerPos[] playersPositions;
    public int playerPosition;

    public class PlayerPos
    {
        public GameObject gameObject;
        public Vector3 position;
        public float distance;
        public Vector3 heading;
        public bool isCurrentPlayer;
        public int lastCheckPoint;
        public int passedCheckpoints;
        public int id;
    }

    public class PositionComparer : IComparer<PlayerPos>
    {
        public int Compare(PlayerPos p1, PlayerPos p2)
        {
            if (p1.passedCheckpoints > p2.passedCheckpoints)
                return -1;
            else if (p1.passedCheckpoints < p2.passedCheckpoints)
                return 1;
            else if (p1.passedCheckpoints == p2.passedCheckpoints)
            {
                if (p1.distance < p2.distance)
                    return -1;
                else if (p1.distance > p2.distance)
                    return 1;
                else
                    return 0;
            }
            else return 0;
        }
    }

    private void Awake()
    {
        playersParent = GameObject.FindWithTag("Cars").transform;
        playersCount = playersParent.childCount;
        checkPointParent = GameObject.FindWithTag("Checkpoint").transform;
        checkPointCount = checkPointParent.childCount;

        checkPointsPosition = new Vector3[checkPointCount];
        playersPositions = new PlayerPos[playersCount];

        for (int i = 0; i < checkPointCount; i++)
        {
            checkPointsPosition[i] = checkPointParent.GetChild(i).GetComponent<Transform>().position;
        }
        CreatePlayerPosData();
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        UpdatePlayerPosData();
        //SmartSort();
        CountCheckpointDistance();
        CheckPlayerPosition();
    }

    private void FixedUpdate()
    {
        SmartSort();
    }

    void CountCheckpointDistance()
    {
        for (int i = 0; i < playersCount; i++)
        {
            if(playersPositions[i].lastCheckPoint == checkPointsPosition.Length)
            {
                playersPositions[i].heading = checkPointsPosition[0] - playersPositions[i].position;
                playersPositions[i].distance = playersPositions[i].heading.magnitude;
                //Debug.Log(playersPositions[i].heading.magnitude);
            }
            else
            {
                playersPositions[i].heading = checkPointsPosition[playersPositions[i].lastCheckPoint] - playersPositions[i].position;
                playersPositions[i].distance = playersPositions[i].heading.magnitude;
                //Debug.Log(playersPositions[i].heading.magnitude);
            }
        }
    }

    void CreatePlayerPosData()
    {
        for (int i = 0; i < playersCount; i++)
        {
            playersPositions[i] = new PlayerPos();
            if(playersParent.GetChild(i).gameObject.CompareTag("Player"))
            {
                playersPositions[i].isCurrentPlayer = true;
            }
            playersPositions[i].position = playersParent.GetChild(i).position;
            playersPositions[i].lastCheckPoint = playersParent.GetChild(i).GetComponent<Player>().lastCheckpoint;
            playersPositions[i].passedCheckpoints = playersParent.GetChild(i).GetComponent<Player>().passedCheckpoints;
            playersPositions[i].id = i;
            playersPositions[i].gameObject = playersParent.GetChild(i).gameObject;
        }
    }

    public void UpdatePlayerPosData()
    {
        for (int i = 0; i < playersCount; i++)
        {
            for (int j = 0; j < playersCount; j++)
            {
                if(playersPositions[i].gameObject == playersParent.GetChild(j).gameObject)
                {
                    playersPositions[i].position = playersParent.GetChild(j).position;
                    playersPositions[i].lastCheckPoint = playersParent.GetChild(j).GetComponent<Player>().lastCheckpoint;
                    playersPositions[i].passedCheckpoints = playersParent.GetChild(j).GetComponent<Player>().passedCheckpoints;
                }
            }
        }
    }

    void SortPlayersByCheckpoint()
    {
        Array.Sort(playersPositions, delegate (PlayerPos p1, PlayerPos p2)  //"OrderBy" more easy?
            { return p1.lastCheckPoint.CompareTo(p2.lastCheckPoint); });    //Simple version of Icomparer Interface
    }

    void SmartSort() 
    {
        Array.Sort(playersPositions,new PositionComparer());
    }

    void CheckPlayerPosition()
    {
        for (int i = 0; i < playersCount; i++)
        {
            if (playersPositions[i].isCurrentPlayer)
            {
                playerPosition = i;
                return;
            }
        }
    }
}