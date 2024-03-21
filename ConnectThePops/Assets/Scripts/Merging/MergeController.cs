using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MergeController : MonoBehaviour
{
    public static MergeController Instance;
        
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    [SerializeField] private GridItemsOnScene gridItemsOnScene;
    [SerializeField] private GridItemsToMerge gridItemsToMerge;
    [SerializeField] private MergeResult mergeResult;
    [SerializeField] private int minimumMergeCount = 2;
    [SerializeField] private AnimationCurve animationCurveBounce;
    private GridItemType gridItemToMerge = null;
    private LineRenderer lineRenderer;
    private int pointNumber;

    public UnityEvent OnMergeFail { get; } = new UnityEvent();
    public UnityEvent OnMergeComplete { get; } = new UnityEvent();
    public UnityEvent<GridItem> OnMerge { get; }  = new UnityEvent<GridItem>();
    public UnityEvent OnDemerge { get; } = new UnityEvent();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ResetLines();
        PressController.Instance.OnRelease.AddListener(ProceedMerge);
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

            gridItem.transform.localScale *= 1.15f;
            SetUpNewLine(gridItem);
            
            OnMerge.Invoke(gridItem);
            CheckMergeResult();
        }
    }

    private void SetUpNewLine(GridItem gridItem)
    {
        lineRenderer.startColor = gridItem.Type.color;
        lineRenderer.endColor = gridItem.Type.color;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(pointNumber, gridItem.transform.position);
        pointNumber++;
    }

    private void ResetLines()
    {
        lineRenderer.positionCount = 0;
        pointNumber = 0;
    }

    public void Split(GridItem gridItem)
    {
        lineRenderer.positionCount--;
        pointNumber--;
        gridItem.transform.localScale /= 1.15f;
        gridItemsToMerge.RemoveFromList(gridItem);
        CheckMergeResult();
        OnDemerge.Invoke();
    }

    private void ProceedMerge()
    {
        if (gridItemsToMerge.Count() < minimumMergeCount)
        {
            MergeFail();
            return;
        }
        FinishMerge();
    }

    private int CheckMergeResult()
    {
        var numberOfSameGridItems = gridItemsToMerge.Count() / 2;
        var numberToSum = gridItemsToMerge.GetLastElement().Type.number;
        int sum = 0;
        for (int i = 0; i < numberOfSameGridItems; i++)
        {
            sum += numberToSum * 2;
        }

        if (sum == 0) sum = numberToSum;
        sum = RoundToNearestPowerOfTwo(sum);

        mergeResult.gameObject.SetActive(true);
        mergeResult.UpdateMergeResult(sum);
        return sum;
    }

    private int RoundToNearestPowerOfTwo(int number)
    {
        if ((number & (number - 1)) == 0 && number != 0)
        {
            return number;
        }

        int powerOfTwo = 1;
        while (powerOfTwo < number)
        {
            powerOfTwo *= 2;
        }

        int lowerPowerOfTwo = powerOfTwo / 2;
        return (number - lowerPowerOfTwo <= powerOfTwo - number) ? lowerPowerOfTwo : powerOfTwo;
    }

    private void FinishMerge()
    {
        gridItemToMerge = null;
        ResetLines();
        mergeResult.gameObject.SetActive(false);
        StartCoroutine(MoveAllItemsToLastItemC());
    }

    private IEnumerator MoveAllItemsToLastItemC()
    {
        var allGridItemsToMerge = new List<GridItem>(gridItemsToMerge.GetAllElements());
        allGridItemsToMerge.Remove(allGridItemsToMerge.Last());
        
        GridItem lastItem = gridItemsToMerge.GetLastElement();
        Vector3 targetPos = lastItem.transform.position;

        List<Vector3> startPositions = new List<Vector3>();
        foreach (var gridItem in allGridItemsToMerge)
        {
            startPositions.Add(gridItem.transform.position);
        }

        float elapsedTime = 0f;
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

        var lastElement = gridItemsToMerge.GetLastElement();
        lastElement.UpdateGridItem(CheckMergeResult());
        StartCoroutine(BounceItemC(lastElement));
        
        foreach (var gridItem in allGridItemsToMerge)
        {
            gridItem.transform.position = targetPos;
            gridItemsOnScene.RemoveFromList(gridItem);
            Destroy(gridItem.gameObject);
        }
        
        gridItemsToMerge.Clear();
        OnMergeComplete.Invoke();
        mergeResult.gameObject.SetActive(false);
        
    }

    private IEnumerator BounceItemC(GridItem item)
    {
        float scaleTime = 0.4f;
        float elapsedScaleTime = 0f;
        while (elapsedScaleTime < scaleTime)
        {
            var bounce = animationCurveBounce.Evaluate(elapsedScaleTime / scaleTime);
            item.transform.localScale = new Vector3(bounce,bounce, 1);
            elapsedScaleTime += Time.deltaTime;
            yield return null;
        }

        item.transform.localScale = Vector3.one;
    }

    public void MergeFail()
    {
        foreach (var gridItem in gridItemsToMerge.GetAllElements())
        {
            gridItem.transform.localScale /= 1.15f;
        }
        
        gridItemToMerge = null;
        OnMergeFail.Invoke();

        gridItemsToMerge.Clear();
        ResetLines();
    }

    private void OnDestroy()
    {
        PressController.Instance.OnRelease.RemoveListener(ProceedMerge);
    }
}
