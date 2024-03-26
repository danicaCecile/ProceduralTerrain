using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileMapController : MonoBehaviour
{
    public Tilemap exampleTilemap; // tile map to be used as an example
    public Tilemap generatedTilemap; // the tile map that will be generated to

    // the below variables store the start and end positions of the example and generated tile maps.
    // the start position refers to the lower left tile and the end position refers to the upper right tile.
    // tile maps must be rectangles.

    public Vector3Int generatedTilemapStartPosition; // the start position of the generated tile map
    public Vector3Int generatedTilemapEndPosition; // the end position of the generated tile map

  //  private List<Rule> rulesList;
    private List<Tile> generatedTilemapListGlobal = new List<Tile>();
    private int generatedTilemapWidth = 0;

    private List<Tile> exampleTilemapListGlobal = new List<Tile>();
    private int exampleTilemapWidth = 0;

    public TileBase defaultTile;

    private System.Random rand = new System.Random();

    //============================== Debug functions ========================================================
    public Tile GetTileAtCoordinates(Vector3Int coordinates)
    {
        Vector2Int offset = generatedTilemapListGlobal[0].position;
        int xOffset = coordinates.x - offset.x;
        int yOffset = coordinates.y - offset.y;
        int index = generatedTilemapWidth * yOffset + xOffset;
        return generatedTilemapListGlobal[index];
    }

    public void PrintNeighborsAtCoordinates(Vector3Int coordinates)
    {
        Tile tile = GetTileAtCoordinates(coordinates);
        Tile northNeighbor = GetNorthNeighbor(tile, generatedTilemapListGlobal, generatedTilemapWidth);
        Tile eastNeighbor = GetEastNeighbor(tile, generatedTilemapListGlobal, generatedTilemapWidth);
        Tile southNeighbor = GetSouthNeighbor(tile, generatedTilemapListGlobal, generatedTilemapWidth);
        Tile westNeighbor = GetWestNeighbor(tile, generatedTilemapListGlobal, generatedTilemapWidth);

        if(northNeighbor != null) Debug.Log($"The north neighbor has the coordinates {northNeighbor.position} and is of type {northNeighbor.type}");
        if(eastNeighbor != null) Debug.Log($"The east neighbor has the coordinates {eastNeighbor.position} and is of type {eastNeighbor.type}");
        if(southNeighbor != null) Debug.Log($"The south neighbor has the coordinates {southNeighbor.position} and is of type {southNeighbor.type}");
        if(westNeighbor != null) Debug.Log($"The west neighbor has the coordinates {westNeighbor.position} and is of type {westNeighbor.type}");
    }

    public void PrintPotentialTypesAtCoordinates(Vector3Int coordinates)
    {
        Tile tile = GetTileAtCoordinates(coordinates);
        List<TileBase> possibleTypes = tile.possibleTypes;
        string output = "Possible types: ";
        foreach(TileBase type in possibleTypes) output = string.Concat(output, $"{type.ToString()} at a {tile.possibleTypesProbability[tile.possibleTypes.IndexOf(type)]} chance");
        Debug.Log(output);
    }

    //---------------- call to start generation -------------------------------------------------------------------
    public void StartGeneration()
    {
        exampleTilemap.CompressBounds();
        GenerateTilemap(exampleTilemap, generatedTilemap, generatedTilemapStartPosition, generatedTilemapEndPosition, defaultTile);
        exampleTilemap.gameObject.SetActive(false);
        generatedTilemap.gameObject.SetActive(true);
    }

    //=========================== Get attributes of a tile map ======================================================================
    private Vector3Int GetStartTilePos(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int startTilePosition = new Vector3Int(bounds.xMin, bounds.yMin, 0);
        return startTilePosition;
    }

    private Vector3Int GetEndTilePos(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int endTilePosition = new Vector3Int(bounds.xMax, bounds.yMax, 0);
        return endTilePosition;
    }

    private int GetWidth(Tilemap tilemap)
    {
        return Mathf.Abs(GetEndTilePos(tilemap).x - GetStartTilePos(tilemap).x);
    }

    private int GetHeight(Tilemap tilemap)
    {
        return Mathf.Abs(GetEndTilePos(tilemap).y - GetStartTilePos(tilemap).y);
    }

    private List<TileBase> GetTileTypesInMap(List<Tile> tilemapList)
    {
        List<TileBase> possibleTileTypes = new List<TileBase>();
        foreach(Tile tile in tilemapList)
        {
            if(possibleTileTypes.Contains(tile.type)) continue;
            possibleTileTypes.Add(tile.type);
        }
        return possibleTileTypes;
    }

    private List<float> GetTilePercentagesInMap(List<Tile> tilemapList)
    {
        List<float> tilePercentagesInMap = new List<float>();
        List<TileBase> processedTileTypes = new List<TileBase>();

        int counter;
        float percentage;

        foreach(Tile tile0 in tilemapList)
        {
            if(processedTileTypes.Contains(tile0.type)) continue;
            counter = 0;
            foreach(Tile tile1 in tilemapList) if(tile1.type == tile0.type) counter++;
            percentage = (float)counter/(float)tilemapList.Count;
            tilePercentagesInMap.Add(percentage);
            //Debug.Log($"{counter} of {tile0.type.ToString()} are in this map. That is {percentage} of the map.");
            processedTileTypes.Add(tile0.type);
        }
        
        return tilePercentagesInMap;
    }

    //==================================== Get attributes of a tile ========================================================================
    private Vector2Int GetColumnRow(int width, int index)
    {
        int indexModWidth = (index+1)%width;

        int column = 0;
        int row = 0;

        if(indexModWidth == 0) 
        {
            column = width;
            row = (index+1)/width;
        }
        else 
        {
            column = (index+1)%width;
            row = (index/width)+1;
        }

        return new Vector2Int(column, row);
    }

    private Vector2Int GetCoordinates(Vector3Int startTilePos, int width, int index)
    {
        Vector2Int rowColumn = GetColumnRow(width, index);
        Vector2Int coordinates = new Vector2Int(rowColumn.x-1, rowColumn.y-1) + new Vector2Int(startTilePos.x, startTilePos.y);
        return coordinates;
    }

    private Tile GetNorthNeighbor(Tile tile, List<Tile> tilemapList, int width)
    {
        Tile northNeighbor = null;
        int tileIndex = tilemapList.IndexOf(tile);
        if(tileIndex + width >= tilemapList.Count) return northNeighbor;
        northNeighbor = tilemapList[tileIndex + width];
        return northNeighbor;
    }

    private Tile GetEastNeighbor(Tile tile, List<Tile> tilemapList, int width)
    {
        Tile eastNeighbor = null;
        if(tile.position.x == tilemapList[width-1].position.x) return eastNeighbor;
        int tileIndex = tilemapList.IndexOf(tile);
        eastNeighbor = tilemapList[tileIndex + 1];
        return eastNeighbor;
    }

    private Tile GetSouthNeighbor(Tile tile, List<Tile> tilemapList, int width)
    {
        Tile southNeighbor = null;
        int tileIndex = tilemapList.IndexOf(tile);
        if(tileIndex - width < 0) return southNeighbor;
        southNeighbor = tilemapList[tileIndex - width];
        return southNeighbor;
    }

    private Tile GetWestNeighbor(Tile tile, List<Tile> tilemapList, int width)
    {
        Tile westNeighbor = null;
        if(tile.position.x == tilemapList[0].position.x) return westNeighbor;
        int tileIndex = tilemapList.IndexOf(tile);
        westNeighbor = tilemapList[tileIndex - 1];
        return westNeighbor;
    }

    private List<Tile> ExampleTilemapListInit(Tilemap tilemap)
    {
        List<Tile> tilemapList = new List<Tile>();

        int width = GetWidth(tilemap);
        exampleTilemapWidth = width;
        int height = GetHeight(tilemap);
        int tileCount = width * height;

        Tile tempTile = null;

        for(int i = 0; i < tileCount; i++)
        {
            tempTile = ScriptableObject.CreateInstance<Tile>();

            tempTile.position = GetCoordinates(GetStartTilePos(tilemap), width, i);
            //Debug.Log(i + ": " + tempTile.position);
            tempTile.type = tilemap.GetTile(new Vector3Int(tempTile.position.x, tempTile.position.y, 0));
            tilemapList.Add(tempTile);
        }
        //Debug.Log(tilemapList.Count + " tiles added to tile list.");
        return tilemapList;
    }

    private bool RulesListContains(Rule targetRule, List<Rule> ruleListTemp)
    {
        foreach(Rule rule in ruleListTemp)
        {
            if(targetRule.target == rule.target && targetRule.neighbor == rule.neighbor && targetRule.direction == rule.direction) return true;
        }
        return false;
    }

    private void PrintRulesList(List<Rule> rulesListTemp)
    {
        foreach(Rule rule in rulesListTemp)
        {
            rule.PrintRule();
        }
    }

    private bool AddRuleToList(Rule rule, List<Rule> rulesListTemp, Tile target, Tile neighbor, int direction)
    {
        rule.target = target.type;
        rule.neighbor = neighbor.type;
        rule.direction = direction;
        if(RulesListContains(rule, rulesListTemp)) return false;
        rulesListTemp.Add(rule);
        return true;
    }

    private bool AddRulePairToList(List<Rule> rulesListTemp, Tile target, Tile neighbor, int direction)
    {
        if(neighbor == null) return false;

        bool isSuccess = false;

        Rule rule0 = ScriptableObject.CreateInstance<Rule>();
        isSuccess = AddRuleToList(rule0, rulesListTemp, target, neighbor, direction);
        ScriptableObject.Destroy(rule0);

        Rule rule1 = ScriptableObject.CreateInstance<Rule>();
        int oppositeDirection = (direction + 2) % 4;
        isSuccess = AddRuleToList(rule1, rulesListTemp, neighbor, target, oppositeDirection);
        ScriptableObject.Destroy(rule1);

        return isSuccess;
    }

    // rules are stored in a list of rules
    // the rule has three values. two are the target tile and the neighbor tile. 
    // the last is a direction. for any rule, the target tile will be direction from the neighbor tile.
    // the directions are as follows: 0 - north, 1 - east, 2 - south, 3 - west 
    private List<Rule> RulesListInit(List<Tile> tilemapList, int width)
    {
        List<Rule> rulesListTemp = new List<Rule>();

        Tile northNeighbor = null;
        Tile eastNeighbor = null;
        Tile southNeighbor = null;
        Tile westNeighbor = null;

        foreach(Tile tile in tilemapList)
        {
            // add north neighbor rules
            northNeighbor = GetNorthNeighbor(tile, tilemapList, width);
            AddRulePairToList(rulesListTemp, tile, northNeighbor, 2);

            // add east neighbor rules
            eastNeighbor = GetEastNeighbor(tile, tilemapList, width);
            AddRulePairToList(rulesListTemp, tile, eastNeighbor, 3);

            // add south neighbor rules
            southNeighbor = GetSouthNeighbor(tile, tilemapList, width);
            AddRulePairToList(rulesListTemp, tile, southNeighbor, 0);

            // add west neighbor rules
            westNeighbor = GetWestNeighbor(tile, tilemapList, width);
            AddRulePairToList(rulesListTemp, tile, westNeighbor, 1);
        }
        //Debug.Log(rulesListTemp.Count);
        //PrintRulesList(rulesListTemp);
        return rulesListTemp;
    }

    private List<Tile> GeneratedTilemapListInit(List<Tile> exampleTilemapList, Vector3Int startTilePos, Vector3Int endTilePos)
    {
        int width = Mathf.Abs(endTilePos.x - startTilePos.x);
        generatedTilemapWidth = width;
        int height = Mathf.Abs(endTilePos.y - startTilePos.y);
        int tileCount = width * height;

        List<Tile> tilemapList = new List<Tile>();
        Tile tempTile = null;
        List<TileBase> possibleTypes = GetTileTypesInMap(exampleTilemapList);
        List<float> possibleTypesProbability = GetTilePercentagesInMap(exampleTilemapList);
        for(int i = 0; i < tileCount; i++)
        {
            tempTile = ScriptableObject.CreateInstance<Tile>();

            tempTile.position = GetCoordinates(startTilePos, width, i);
            tempTile.type = null;
            tempTile.possibleTypes = possibleTypes;
            tempTile.possibleTypesProbability = possibleTypesProbability;
            tilemapList.Add(tempTile);
        }

        return tilemapList;
    }

    private void PrintTileBaseList(List<TileBase> tileBaseList)
    {
        string output = "Tile bases in list: ";
        foreach(TileBase tileBase in tileBaseList) output = string.Concat(output, $"{tileBase.ToString()} ");
        Debug.Log(output);
    }

    //this function will update all of the neighbors' possible tile types
    //returns false if there is a contridiction and returns true if there is no contridiction
    private bool UpdateNeighbor(Tile tile, List<Tile> tilemapList, int width, List<Rule> rulesList)
    {
        Tile northNeighbor = GetNorthNeighbor(tile, tilemapList, width);
        Tile eastNeighbor = GetEastNeighbor(tile, tilemapList, width);
        Tile southNeighbor = GetSouthNeighbor(tile, tilemapList, width);
        Tile westNeighbor = GetWestNeighbor(tile, tilemapList, width);

        if(tile.hasCollapsed == false)
        {
            List<TileBase> northNeighborTiles = new List<TileBase>();
            List<TileBase> eastNeighborTiles = new List<TileBase>();
            List<TileBase> southNeighborTiles = new List<TileBase>();
            List<TileBase> westNeighborTiles = new List<TileBase>();

            foreach(Rule rule in rulesList)
            {
                if(northNeighbor != null)
                {
                    if(rule.direction == 0 && (northNeighbor.possibleTypes.Contains(rule.target) || northNeighbor.type == rule.target)) northNeighborTiles.Add(rule.neighbor);
                }

                if(eastNeighbor != null)
                {
                    if(rule.direction == 1 && (eastNeighbor.possibleTypes.Contains(rule.target) || eastNeighbor.type == rule.target)) eastNeighborTiles.Add(rule.neighbor);
                }

                if(southNeighbor != null)
                {
                    if(rule.direction == 2 && (southNeighbor.possibleTypes.Contains(rule.target) || southNeighbor.type == rule.target)) southNeighborTiles.Add(rule.neighbor);
                }

                if(westNeighbor != null)
                {
                    if(rule.direction == 3 && (westNeighbor.possibleTypes.Contains(rule.target) || westNeighbor.type == rule.target)) westNeighborTiles.Add(rule.neighbor);
                }
            }

            List<TileBase> northSouthIntersect = northNeighborTiles;
            if(northNeighborTiles.Count != 0 && southNeighborTiles.Count != 0) northSouthIntersect = northNeighborTiles.Intersect(southNeighborTiles).ToList();
            else if(northNeighborTiles.Count == 0) northSouthIntersect = southNeighborTiles;

            /*Debug.Log("North neighbor");
            PrintTileBaseList(northNeighborTiles);
            Debug.Log("South neighbor");
            PrintTileBaseList(southNeighborTiles);
            Debug.Log("Intersect");
            PrintTileBaseList(northSouthIntersect);*/

            List<TileBase> eastWestIntersect = eastNeighborTiles;
            if(eastNeighborTiles.Count != 0 && westNeighborTiles.Count != 0) eastWestIntersect = eastNeighborTiles.Intersect(westNeighborTiles).ToList();
            else if(eastNeighborTiles.Count == 0) eastWestIntersect = westNeighborTiles;

            List<TileBase> allNeighborsIntersect = northSouthIntersect;
            if(northSouthIntersect.Count != 0 && eastWestIntersect.Count != 0) allNeighborsIntersect = northSouthIntersect.Intersect(eastWestIntersect).ToList();
            else if(northSouthIntersect.Count == 0) allNeighborsIntersect = eastWestIntersect;

            if(allNeighborsIntersect.Count == 0) return false;
            tile.UpdatePossibleTypes(allNeighborsIntersect);
            //tile.possibleTypes = allNeighborsIntersect;
        }
        tile.hasUpdated = true;
        //Debug.Log("Intersect of all neighbors");
        //PrintTileBaseList(allNeighborsIntersect);
        if(northNeighbor != null && northNeighbor.hasUpdated == false) UpdateNeighbor(northNeighbor, tilemapList, width, rulesList);
        if(eastNeighbor != null && eastNeighbor.hasUpdated == false) UpdateNeighbor(eastNeighbor, tilemapList, width, rulesList);
        if(southNeighbor != null && southNeighbor.hasUpdated == false) UpdateNeighbor(southNeighbor, tilemapList, width, rulesList);
        if(westNeighbor != null && westNeighbor.hasUpdated == false) UpdateNeighbor(westNeighbor, tilemapList, width, rulesList);

        return true;
    }

    private bool CollapseTile(List<Tile> tilemapList, int index, int width, Tilemap tilemap, List<Rule> rulesList)
    {
        bool noContridiction = true;
        Tile tempTile = tilemapList[index];
        TileBase chosenType = tempTile.SetRandomType();
        tilemap.SetTile(new Vector3Int(tempTile.position.x, tempTile.position.y, 0), chosenType);

        tempTile.possibleTypes = new List<TileBase>();
        tempTile.hasCollapsed = true;

        Tile northNeighbor = GetNorthNeighbor(tempTile, tilemapList, width);
        Tile eastNeighbor = GetEastNeighbor(tempTile, tilemapList, width);
        Tile southNeighbor = GetSouthNeighbor(tempTile, tilemapList, width);
        Tile westNeighbor = GetWestNeighbor(tempTile, tilemapList, width);

        bool northNeighborContridiction = true;
        bool eastNeighborContridiction = true;
        bool southNeighborContridiction = true;
        bool westNeighborContridiction = true;

        if(northNeighbor != null) northNeighborContridiction = UpdateNeighbor(northNeighbor, tilemapList, width, rulesList);
        if(eastNeighbor != null) eastNeighborContridiction = UpdateNeighbor(eastNeighbor, tilemapList, width, rulesList);
        if(southNeighbor != null) southNeighborContridiction = UpdateNeighbor(southNeighbor, tilemapList, width, rulesList);
        if(westNeighbor != null) westNeighborContridiction = UpdateNeighbor(westNeighbor, tilemapList, width, rulesList);

        foreach(Tile tile in tilemapList) tile.hasUpdated = false;

        noContridiction = !northNeighborContridiction || !eastNeighborContridiction || !southNeighborContridiction || !westNeighborContridiction;
        //Debug.Log(noContridiction);
        return !noContridiction;
    }

    private void FillTilemapFromList(Tilemap tilemap, List<Tile> tilemapList, TileBase tileBase)
    {
        Vector3Int tilePosition = new Vector3Int(0, 0, 0);
        foreach(Tile tile in tilemapList)
        {
            tilePosition = new Vector3Int(tile.position.x, tile.position.y, 0);
            tilemap.SetTile(tilePosition, tileBase);
        }
    }

    private bool IsWavefunctionCollapsed(List<Tile> tilemapList)
    {
        foreach(Tile tile in tilemapList) if(tile.hasCollapsed == false) return false;
        return true;
    }

    private List<Tile> GetLowestEntropyTiles(List<Tile> tilemapList)
    {
        int smallestCount = int.MaxValue;
        List<Tile> lowestEntropyTiles = new List<Tile>();

        foreach(Tile tile in tilemapList)
        {
            if(tile.possibleTypes.Count == 0) continue;
            if(tile.possibleTypes.Count == smallestCount) lowestEntropyTiles.Add(tile);
            else if(tile.possibleTypes.Count < smallestCount)
            {
                smallestCount = tile.possibleTypes.Count;
                lowestEntropyTiles = new List<Tile>();
                lowestEntropyTiles.Add(tile);
            }
        }
        return lowestEntropyTiles;
    }

    private void GenerateTilemap(Tilemap exampleTilemapTemp, Tilemap generatedTilemapTemp, Vector3Int startTilePos, Vector3Int endTilePos, TileBase fillTile)
    {
        List<Tile> exampleTilemapList = ExampleTilemapListInit(exampleTilemapTemp);
        exampleTilemapListGlobal = exampleTilemapList;
        List<Tile> generatedTilemapList = GeneratedTilemapListInit(exampleTilemapList, startTilePos, endTilePos);
        generatedTilemapListGlobal = generatedTilemapList;

        FillTilemapFromList(generatedTilemapTemp, generatedTilemapList, fillTile);
        List<Rule> rulesList = RulesListInit(exampleTilemapList, GetWidth(exampleTilemap));

        //PrintRulesList(rulesList);
        
        int randomIndex = rand.Next(0, generatedTilemapList.Count);
        CollapseTile(generatedTilemapList, randomIndex, Mathf.Abs(endTilePos.x - startTilePos.x), generatedTilemap, rulesList);

        List<Tile> lowestEntropyTiles;
        int nextIndex;

        while(IsWavefunctionCollapsed(generatedTilemapList) == false)
        {
            lowestEntropyTiles = GetLowestEntropyTiles(generatedTilemapList);
            randomIndex = rand.Next(0, lowestEntropyTiles.Count);
            nextIndex = generatedTilemapList.IndexOf(lowestEntropyTiles[randomIndex]);
            if(CollapseTile(generatedTilemapList, nextIndex, Mathf.Abs(endTilePos.x - startTilePos.x), generatedTilemap, rulesList) == false) break;
            //CollapseTile(generatedTilemapList, nextIndex, Mathf.Abs(endTilePos.x - startTilePos.x), generatedTilemap, rulesList);
        }
    }
}
