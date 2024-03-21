using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MergeController : MonoBehaviour
{
    [SerializeField] private GridItemsOnScene gridItemsOnScene;
    [SerializeField] private GridItemsToMerge gridItemsToMerge;
    [SerializeField] private MergeResult mergeResult;
    [SerializeField] private int minimumMergeCount = 2;
    [SerializeField] private AnimationCurve animationCurveBounce;
    [SerializeField] private float scaleWhenSelected = 1.15f;
    private GridItemType gridItemToMerge = null;
    private LineRenderer lineRenderer;
    private int numberOfLines;
    
    public static MergeController Instance;
        
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    public UnityEvent OnMergeComplete { get; } = new UnityEvent();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ResetLines();
        PressController.Instance.OnRelease.AddListener(TryToFinishMerge);
    }

    public void Merge(GridItem gridItem)
    {
        if (gridItem == null) return;
        if (gridItemToMerge == null) gridItemToMerge = gridItem.Type;
        if (SpawnPointController.Instance.GetSpawnPointByGroundPosition(gridItem.Ground).IsActive == false) return;
        if (gridItemsToMerge.Count() > 0 && SpawnPointController.Instance
            .IsSelectedBubbleNeighborWith(gridItem.Ground, gridItemsToMerge.GetLastElement().Ground) == false) return;

        if (gridItem.Type == gridItemToMerge)
        {
            if (gridItemsToMerge.Contains(gridItem)) return;
            
            gridItemsToMerge.AddToList(gridItem);
            gridItem.transform.localScale *= scaleWhenSelected;
            SetUpNewLine(gridItem);
            
            CheckMergeResult();
        }
    }
    
    private void TryToFinishMerge()
    {
        if (gridItemsToMerge.Count() < minimumMergeCount)
        {
            MergeFail();
            return;
        }
        FinishMerge();
    }
    
    private void FinishMerge()
    {
        gridItemToMerge = null;
        ResetLines();
        mergeResult.gameObject.SetActive(false);
        StartCoroutine(MoveAllItemsInChainToLastItemC());
    }
    
    public void Split(GridItem gridItem)
    {
        lineRenderer.positionCount--;
        numberOfLines--;
        gridItem.transform.localScale /= scaleWhenSelected;
        gridItemsToMerge.RemoveFromList(gridItem);
        CheckMergeResult();
    }

    private void SetUpNewLine(GridItem gridItem)
    {
        lineRenderer.startColor = gridItem.Type.color;
        lineRenderer.endColor = gridItem.Type.color;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(numberOfLines, gridItem.transform.position);
        numberOfLines++;
    }

    private void ResetLines()
    {
        lineRenderer.positionCount = 0;
        numberOfLines = 0;
    }
    
    private int CheckMergeResult()
    {
        var numberOfSameGridItems = gridItemsToMerge.Count() / 2;
        var numberToSum = gridItemsToMerge.GetLastElement().Type.number;
        
        var calculatedMergeResult = 0;
        for (int i = 0; i < numberOfSameGridItems; i++)
        {
            calculatedMergeResult += numberToSum * 2;
        }

        if (calculatedMergeResult == 0) calculatedMergeResult = numberToSum;
        calculatedMergeResult = RoundToNearestPowerOfTwo(calculatedMergeResult);

        mergeResult.gameObject.SetActive(true);
        mergeResult.UpdateMergeResult(calculatedMergeResult);
        return calculatedMergeResult;
    }

    private int RoundToNearestPowerOfTwo(int number)
    {
        if ((number & (number - 1)) == 0 && number != 0)
        {
            return number;
        }

        var powerOfTwo = 1;
        while (powerOfTwo < number)
        {
            powerOfTwo *= 2;
        }

        var lowerPowerOfTwo = powerOfTwo / 2;
        return (number - lowerPowerOfTwo <= powerOfTwo - number) ? lowerPowerOfTwo : powerOfTwo;
    }

    private IEnumerator MoveAllItemsInChainToLastItemC()
    {
        var allGridItemsToMerge = new List<GridItem>(gridItemsToMerge.GetAllElements());
        allGridItemsToMerge.Remove(allGridItemsToMerge.Last());
        
        var lastItem = gridItemsToMerge.GetLastElement();
        var targetPos = lastItem.transform.position;

        var startPositions = new List<Vector3>();
        foreach (var gridItem in allGridItemsToMerge)
        {
            startPositions.Add(gridItem.transform.position);
        }

        var elapsedTime = 0f;
        var duration = 0.2f;

        while (elapsedTime < duration)
        {
            for (int i = 0; i < allGridItemsToMerge.Count(); i++)
            {
                allGridItemsToMerge[i].transform.position = Vector3.Lerp(startPositions[i], targetPos, elapsedTime / duration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        UpdateLastElementInChain();
        RemoveElementsFromChain(allGridItemsToMerge);
    }

    private void UpdateLastElementInChain()
    {
        var lastElement = gridItemsToMerge.GetLastElement();
        lastElement.UpdateGridItem(CheckMergeResult());
        StartCoroutine(BounceItemC(lastElement));
    }

    private void RemoveElementsFromChain(List<GridItem> allGridItemsToMerge)
    {
        foreach (var gridItem in allGridItemsToMerge)
        {
            gridItemsOnScene.RemoveFromList(gridItem);
            Destroy(gridItem.gameObject);
        }
        
        gridItemsToMerge.Clear();
        OnMergeComplete.Invoke();
        mergeResult.gameObject.SetActive(false);
    }

    private IEnumerator BounceItemC(GridItem item)
    {
        var scaleTime = 0.4f;
        var elapsedScaleTime = 0f;
        while (elapsedScaleTime < scaleTime)
        {
            var bounce = animationCurveBounce.Evaluate(elapsedScaleTime / scaleTime);
            item.transform.localScale = new Vector3(bounce,bounce, 1);
            elapsedScaleTime += Time.deltaTime;
            yield return null;
        }

        item.transform.localScale = Vector3.one;
    }

    private void MergeFail()
    {
        foreach (var gridItem in gridItemsToMerge.GetAllElements())
        {
            gridItem.transform.localScale /= scaleWhenSelected;
        }
        
        gridItemToMerge = null;

        gridItemsToMerge.Clear();
        ResetLines();
    }

    private void OnDestroy()
    {
        PressController.Instance.OnRelease.RemoveListener(TryToFinishMerge);
    }
}
