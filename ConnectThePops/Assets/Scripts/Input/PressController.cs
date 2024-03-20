using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PressController : MonoBehaviour
{
    public static PressController Instance;

    [SerializeField] private GridItemsToMerge gridItemsToMerge;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    [SerializeField] private Camera camera;
    private bool isPressed = false;
    public UnityEvent OnRelease { get; }  = new UnityEvent();
    internal bool IsPressed { get => isPressed; set => isPressed = value; }

    private void Update()
    {
        if (IsPressed)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("GridItem"))
                {
                    Transform objectHit = hit.transform;
                    if (gridItemsToMerge.Count() > 1 && objectHit
                        == gridItemsToMerge.GetBeforeLastElementForDemerge().transform)
                        MergeController.Instance.Demerge(gridItemsToMerge.GetLastElement());
                    else
                        MergeController.Instance.Merge(objectHit.GetComponent<GridItem>());
                        
                }
            }
        }
    }

    public void PressedDown()
    {
        IsPressed = true;
    }

    public void PressedUp()
    {
        IsPressed = false;
        OnRelease.Invoke();
    }
}
