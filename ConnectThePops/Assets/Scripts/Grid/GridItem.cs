using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private TextMeshPro numberText;
    [SerializeField] private GridItemTypes gridItemTypes;
    [SerializeField] private GameObject frame;

    private GridItemType myType;

    public GridItemType Type
    {
        get => myType;
        set => myType = value;
    }

    private Vector3 groundPosition;
    private bool isMoving = false;

    public bool IsMoving
    {
        get => isMoving;
        set => isMoving = value;
    }

    public Vector3 Ground
    {
        get => groundPosition;
        set => groundPosition = value;
    }

    public void InitGridItem(GridItemType type, Vector3 position)
    {
        background.color = type.color;
        numberText.text = CheckIfShouldAbbreviateNumber(type.number);
        groundPosition = position;
        myType = type;
    }
    
    public void UpdateGridItem(int number)
    {
        var type = gridItemTypes.GetAllGridIdemTypes().Find(x => x.number == number);
        background.color = type.color;
        numberText.text = CheckIfShouldAbbreviateNumber(type.number);
        myType = type;
    }
    
    string CheckIfShouldAbbreviateNumber(int number)
    {
        if (number >= 1000000000)
        {
            frame.SetActive(true);
            return (number / 1000000000).ToString() + "B";
        }
        else if (number >= 1000000)
        {
            frame.SetActive(true);
            return (number / 1000000).ToString() + "M";
        }
        else if (number >= 1000)
        {
            frame.SetActive(true);
            return (number / 1000).ToString() + "K";
        }
        else
        {
            frame.SetActive(false);
            return number.ToString();
        }
    }
}
