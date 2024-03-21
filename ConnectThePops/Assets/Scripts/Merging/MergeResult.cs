using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MergeResult : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private TextMeshPro numberText;
    [SerializeField] private GridItemTypes gridItemTypes;
    
    public void UpdateMergeResult(int number)
    {
        var color = gridItemTypes.GetAllGridIdemTypes().Find(x => x.number == number).color;
        background.color = color;
        numberText.text = number.ToString();
    }
}
