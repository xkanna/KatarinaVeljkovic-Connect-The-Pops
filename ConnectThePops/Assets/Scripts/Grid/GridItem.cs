using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private TextMeshPro numberText;

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
        numberText.text = type.number.ToString();
        groundPosition = position;
        myType = type;
    }
}
