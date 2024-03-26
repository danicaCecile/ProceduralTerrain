using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDrawer : MonoBehaviour
{
    public Tilemap targetTilemap;
    private TileBase tileBase;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = targetTilemap.WorldToCell(mouseWorldPos);

            // Change the tile at the clicked position
            targetTilemap.SetTile(cellPosition, tileBase);
        }
    }

    public void SetTilePen(TileBase newTileBase)
    {  
        tileBase = newTileBase;
    }
}
