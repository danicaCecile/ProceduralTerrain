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
                directionString = "north of";
                break;
            case 1:
                directionString = "west of";
                break;
            case 2:
                directionString = "south of";
                break;
            case 3:
                directionString = "east of";
                break;
            case 4:
                directionString = "below";
                break;
            case 5:
                directionString = "above";
                break;
        }
        if(target == null) Debug.Log("Null can be " + directionString + " " + neighbor.ToString());
        else if(neighbor == null) Debug.Log(target.ToString() + " can be " + directionString + " null");
        else Debug.Log(target.ToString() + " can be " + directionString + " of " + neighbor.ToString());
    }
}
