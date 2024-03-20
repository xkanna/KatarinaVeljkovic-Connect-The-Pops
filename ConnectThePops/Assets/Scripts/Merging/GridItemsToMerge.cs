using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GridItemsToMerge", fileName = "GridItemsToMerge", order = 0)]
public class GridItemsToMerge : ScriptableObject
{
    private List<GridItem> gridItems = new List<GridItem>();

    private void OnEnable()
    {
        gridItems.Clear();
    }

    public void AddToList(GridItem bubble)
    {
        if (gridItems.Contains(bubble) == false)
            gridItems.Add(bubble);
    }

    public void RemoveFromList(GridItem bubble)
    {
        if (gridItems.Contains(bubble))
            gridItems.Remove(bubble);
    }

    public void Clear()
    {
        gridItems.Clear();
    }

    public int Count()
    {
        return gridItems.Count;
    }

    public GridItem GetLastElement()
    {
        return gridItems.Last();
    }
    public GridItem GetPenultimateElement()
    {
        return gridItems[gridItems.Count - 1];
    }

    public GridItem GetBeforeLastElementForDemerge()
    {
        return gridItems[gridItems.Count - 2];
    }

    public List<GridItem> GetAllElements()
    {
        return gridItems;
    }

    public GridItemType GetMergeType()
    {
        if (gridItems.Count == 0) return null;
        return gridItems[0].Type;
    }

    public bool Contains(GridItem bubble)
    {
        return gridItems.Contains(bubble);
    }
}
