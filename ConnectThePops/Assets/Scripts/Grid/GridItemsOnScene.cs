using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridItemsOnScene", fileName = "GridItemsOnScene", order = 0)]
public class GridItemsOnScene : ScriptableObject
{
    private List<GridItem> gridItems = new List<GridItem>();
    public Event OnChanged { get; }

    private void OnEnable()
    {
        gridItems.Clear();
    }

    public void AddToList(GridItem gridItem)
    {
        if (gridItems.Contains(gridItem) == false)
            gridItems.Add(gridItem);
        //OnChanged.Invoke();
    }

    public void RemoveFromList(GridItem gridItem)
    {
        if (gridItems.Contains(gridItem))
            gridItems.Remove(gridItem);
        //OnChanged.Invoke();
    }

    public void Clear()
    {
        gridItems.Clear();
    }

    public int Count()
    {
        return gridItems.Count;
    }

    public List<GridItem> GetAllElements()
    {
        return gridItems;
    }
}
