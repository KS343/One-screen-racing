using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBuilder : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(10, 10);
    private Prop[,] grid;
    private Prop currentProp;
    private Camera cam;

    private void Awake()
    {
        grid = new Prop[gridSize.x, gridSize.y];
        cam = Camera.main;
    }

    void Update()
    {
        if(currentProp != null)
        {
            Plane groundPLane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(groundPLane.Raycast(ray,out float position))
            {
                Vector3 worldPostition = ray.GetPoint(position);
                int x = Mathf.RoundToInt(worldPostition.x);
                int z = Mathf.RoundToInt(worldPostition.z);

                bool available = true;

                if (x < 0 || x > gridSize.x - currentProp.size.x) available = false;
                if (z < 0 || z > gridSize.y - currentProp.size.y) available = false;
                if (available && IsPlaceTaken(x, z)) available = false;

                currentProp.transform.position = new Vector3(x, 0, z);
                currentProp.SetTransparent(available);

                if (Input.GetMouseButtonDown(0) && available)
                {
                    PlaceCurrentProp(x, z);
                }
            }
        }
    }

    private bool IsPlaceTaken(int cordx, int cordy)
    {
        for (int x = 0; x < currentProp.size.x; x++)
        {
            for (int y = 0; y < currentProp.size.y; y++)
            {
               if (grid[cordx + x, cordy + y] !=null) return true;
            }
        }
        return false;
    }

    private void PlaceCurrentProp(int cordx,int cordy)
    {
        for (int x = 0; x < currentProp.size.x; x++)
        {
            for (int y = 0; y < currentProp.size.y; y++)
            {
                grid[cordx + x, cordy + y] = currentProp;
            }
        }
        currentProp.SetNormal();
        currentProp = null;
    }

    public void StartPlacingProp(Prop propPrefab)
    {
        if(currentProp != null)
        {
            Destroy(currentProp.gameObject);
        }
        currentProp = Instantiate(propPrefab);
    }

    public void Rotate()
    {
        int buff;
        if(currentProp !=null)
        {
            currentProp.GetComponentInChildren<Transform>().Rotate(0f, 90f, 0f, Space.World);
            buff = currentProp.size.x;
            currentProp.size.x = currentProp.size.y;
            currentProp.size.y = buff;
            if (currentProp.transform.rotation.y == 90f)
            {
                currentProp.GetComponentInChildren<Transform>().localPosition.Set(-0.5f, 0, -0.5f);
            }
            else if (currentProp.transform.rotation.y == 180f)
            {
                currentProp.GetComponentInChildren<Transform>().localPosition.Set(-0.5f, 0, -1.5f);
            }
        }

    }
}
