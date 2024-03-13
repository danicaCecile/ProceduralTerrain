using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDebug : MonoBehaviour
{
    public Tilemap tilemap;
    public TileMapController tilemapController;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            //Debug.Log("Clicked on tile position: " + cellPosition);
            Debug.Log("Calculated tile posiiton: " + tilemapController.GetTileAtCoordinates(cellPosition).position);
            tilemapController.PrintNeighborsAtCoordinates(cellPosition);
            if(tilemapController.GetTileAtCoordinates(cellPosition).type != null) Debug.Log("Clicked on tile type: " + tilemapController.GetTileAtCoordinates(cellPosition).type.ToString());
            tilemapController.PrintPotentialTypesAtCoordinates(cellPosition);
        }
    }
}
