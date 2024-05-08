using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMouseMovement : MonoBehaviour
{
    public Pathfinder pathfinder;
    public TileMapController tilemapController;
    public Tilemap tilemap;
    public TileBase tile0;
    public TileBase tile1;
    public TileBase ogTile0;
    public TileBase ogTile1;

    private int tileBeingSelected = 0;
    private Vector3Int tile0pos;
    private Vector3Int tile1pos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && tilemapController.isGenerated != false) // Left mouse button clicked
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);
            Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int(cellPosition.x, cellPosition.y, 0));
            Debug.Log("Calculated tile positon: " + cellPosition);

            if(tileBeingSelected == 0)
            {
                tile0pos = cellPosition;
                tileBeingSelected++;

                ogTile0 = tilemap.GetTile(cellPosition);
                tilemap.SetTile(cellPosition, tile0);
            }
            else if(tileBeingSelected == 1)
            {
                tile1pos = cellPosition;
                tileBeingSelected++;

                ogTile1 = tilemap.GetTile(cellPosition);
                tilemap.SetTile(cellPosition, tile1);
            }
        }
    }

    public void MoveToLocation()
    {
        tilemapController.isGenerated = false;
        pathfinder.startPosition = new Vector2Int(tile0pos.x, tile0pos.y);
        pathfinder.endPosition = new Vector2Int(tile1pos.x, tile1pos.y);
        pathfinder.followPath();

        tilemap.SetTile(tile0pos, ogTile0);
        tilemap.SetTile(tile1pos, ogTile1);
        tileBeingSelected = 0;
    }
}
