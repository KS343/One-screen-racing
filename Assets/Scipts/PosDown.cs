using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosDown : MonoBehaviour
{
    public Player player;
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CarPos")
        {
            //make for check for last one
            {
                player.carPos++;
                print(player.carPos);
            }
        }
    }
}
