using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "GridItemTypes", fileName = "GridItemTypes", order = 0)]
public class GridItemTypes : ScriptableObject
{
    [SerializeField] private int allowedNumbersToSpawnRandomly;
    [SerializeField] private List<GridItemType> allGridItemTypes;

    public List<GridItemType> GetAllGridIdemTypes()
    {
        return allGridItemTypes;
    }

    public GridItemType GetRandomItemType()
    {
        var randomIndex = Random.Range(0, allowedNumbersToSpawnRandomly - 1);
        return allGridItemTypes[randomIndex];
    }
}
