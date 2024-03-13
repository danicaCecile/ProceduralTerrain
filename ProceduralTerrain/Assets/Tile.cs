using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Tile : ScriptableObject
{
    public Tile northNeighbor;
    public Tile eastNeighbor;
    public Tile southNeighbor;
    public Tile westNeighbor;

    public Vector2Int position;

    public TileBase type;

    public List<TileBase> possibleTypes = new List<TileBase>();
    public List<float> possibleTypesProbability = new List<float>();

    public bool hasUpdated = false;
    public bool hasCollapsed = false;

    private System.Random rand = new System.Random();

    public void UpdatePossibleTypes(List<TileBase> newPossibleTypes)
    {
        possibleTypesProbability = CalculateNewProbabilities(newPossibleTypes);
        possibleTypes = newPossibleTypes;
    }

    public List<float> CalculateNewProbabilities(List<TileBase> newPossibleTypes)
    {
        // Step 1: Calculate the total probability of all tiles in the original list
        float totalProbability = 0f;
        foreach (float probability in possibleTypesProbability)
        {
            totalProbability += probability;
        }

        // Step 2: Calculate the total probability of tiles remaining in the new list
        float newTotalProbability = 0f;
        foreach (TileBase tile in newPossibleTypes)
        {
            int index = possibleTypes.IndexOf(tile);
            if (index != -1) // Tile exists in the original list
            {
                newTotalProbability += possibleTypesProbability[index];
            }
        }

        // Step 3: Adjust probabilities proportionally based on the removal of tiles
        List<float> newPossibleTypesProbability = new List<float>();
        foreach (TileBase tile in newPossibleTypes)
        {
            int index = possibleTypes.IndexOf(tile);
            if (index != -1) // Tile exists in the original list
            {
                float probability = possibleTypesProbability[index] * (totalProbability / newTotalProbability);
                newPossibleTypesProbability.Add(probability);
            }
            else
            {
                // If the tile is not found in the original list, assign zero probability
                newPossibleTypesProbability.Add(0f);
            }
        }

        return newPossibleTypesProbability;
    }

    public TileBase SetRandomType() 
    {
        float totalProbability = 0f;
        foreach (float probability in possibleTypesProbability) totalProbability += probability;

        float randomValue = (float)rand.NextDouble() * totalProbability;

        float cumulativeProbability = 0f;
        int index = 0;
        for(int i = 0; i < possibleTypesProbability.Count; i++)
        {
            cumulativeProbability += possibleTypesProbability[i];
            if(randomValue <= cumulativeProbability)
            {
                index = i;
                break;
            }
        }

        type = possibleTypes[index];

        return type;
    }
}
