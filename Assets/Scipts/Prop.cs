using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Renderer mainRenderer;
    public Vector2Int size = Vector2Int.one;

    public void SetTransparent(bool available)
    {
        if (available)
        {
            
        }
        else
        {
        }
    }

    public void SetNormal()
    {

    }

    /*private void OnDrawGizmos()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Gizmos.color = new Color(0,1,0,0.25f);
                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, 0.1f, 1));
            }
        }
    }*/
}
