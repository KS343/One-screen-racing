using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosUp : MonoBehaviour
{
    public Player player;
    private void OnTriggerExit(Collider other)
    {
        if(other.tag =="CarPos")
        {
            if(player.carPos <=1)
            {
                player.carPos = 1;
            }
            else
            {
                player.carPos--;
            }
            print(player.carPos);
        }
    }
}
