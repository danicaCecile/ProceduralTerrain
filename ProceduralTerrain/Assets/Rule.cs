using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Rule : ScriptableObject
{
    public TileBase target;
    public TileBase neighbor;
    public int direction;

    public void PrintRule()
    {
        string directionString = "";

        switch(direction)
        {
            case 0:
                directionString = "north";
                break;
            case 1:
                directionString = "west";
                break;
            case 2:
                directionString = "south";
                break;
            case 3:
                directionString = "east";
                break;
        }
        Debug.Log(target.ToString() + " can be " + directionString + " of " + neighbor.ToString());
    }
}
