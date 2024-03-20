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

    [SerializeField] private int minimumMergeCount = 2;
    private GridItemType gridItemToMerge = null;
    private float pitch = 1f;

    public UnityEvent OnMergeFail { get; } = new UnityEvent();
    public UnityEvent OnMergeComplete { get; } = new UnityEvent();
    public UnityEvent<GridItem> OnMerge { get; }  = new UnityEvent<GridItem>();
    public UnityEvent OnDemerge { get; } = new UnityEvent();

    private void Start()
    {
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

            gridItem.transform.localScale *= 1.2f;
            OnMerge.Invoke(gridItem);
        }
    }

    public void Demerge(GridItem gridItem)
    {
        gridItem.transform.localScale /= 1.2f;
        gridItemsToMerge.RemoveFromList(gridItem);
        OnDemerge.Invoke();
    }

    public void ProceedMerge()
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
        foreach (var gridItem in gridItemsToMerge.GetAllElements())
        {
            gridItem.transform.localScale /= 1.2f;
            gridItemsOnScene.RemoveFromList(gridItem);
            Destroy(gridItem.gameObject);
        }
        gridItemsToMerge.Clear();
        OnMergeComplete.Invoke();
    }

    public void MergeFail()
    {
        foreach (var gridItem in gridItemsToMerge.GetAllElements())
        {
            gridItem.transform.localScale /= 2;
        }
        
        gridItemToMerge = null;
        OnMergeFail.Invoke();

        gridItemsToMerge.Clear();
    }

    private void OnDestroy()
    {
        PressController.Instance.OnRelease.RemoveListener(ProceedMerge);
    }
}
