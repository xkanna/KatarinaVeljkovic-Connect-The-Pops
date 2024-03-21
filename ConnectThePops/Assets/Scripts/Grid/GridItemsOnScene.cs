using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GridItemsOnScene", fileName = "GridItemsOnScene", order = 0)]
public class GridItemsOnScene : ScriptableObject
{
    private List<GridItem> gridItems = new List<GridItem>();

    private void OnEnable()
    {
        gridItems.Clear();
    }

    public void AddToList(GridItem gridItem)
    {
        if (gridItems.Contains(gridItem) == false)
            gridItems.Add(gridItem);
    }

    public void RemoveFromList(GridItem gridItem)
    {
        if (gridItems.Contains(gridItem))
            gridItems.Remove(gridItem);
    }

    public void Clear()
    {
        gridItems.Clear();
    }

    public List<GridItem> GetAllElements()
    {
        return gridItems;
    }
}
