using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI Lap;
    public TextMeshProUGUI CurrentLap;
    public TextMeshProUGUI LastLap;
    public TextMeshProUGUI BestLap;

    public TextMeshProUGUI CurrentPos;

    public Player player;
    public PositionManager positionManager;

    private int lap;
    private float currentLapTime;
    private float lastLapTime;
    private float bestLap;

    private int currentPosInt;

    private void Start()
    {
        positionManager = GameObject.FindObjectOfType<PositionManager>();
    }

    private void LateUpdate()
    {
        if(player == null)
        {
            return;
        }
        if(player.currentLap != lap)
        {
            lap = player.currentLap;
            Lap.text = lap.ToString();
        }
        if (player.currentLapTime != currentLapTime)
        {
            currentLapTime = player.currentLapTime;
            CurrentLap.text = currentLapTime.ToString();
        }
        LastLap.text = player.lastLapTime.ToString();
        if (player.bestLapTime != bestLap)
        {
            bestLap = player.bestLapTime;
            BestLap.text = bestLap.ToString();
        }
        if (positionManager == null)
        {
            return;
        }
        if(currentPosInt != positionManager.playerPosition)
        {
            currentPosInt = positionManager.playerPosition;
            CurrentPos.text = currentPosInt.ToString();
        }
        
    }
}
