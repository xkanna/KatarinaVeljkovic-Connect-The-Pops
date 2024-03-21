using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressController : MonoBehaviour
{
    [SerializeField] private GridItemsToMerge gridItemsToMerge;
    [SerializeField] private Camera camera;
    private bool isPressed = false;
    
    public static PressController Instance;
    public UnityEvent OnRelease { get; }  = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (isPressed)
        {
            RaycastHit hit;
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("GridItem"))
                {
                    var objectHit = hit.transform;
                    if (gridItemsToMerge.Count() > 1 && objectHit == gridItemsToMerge.GetBeforeLastElementForDemerge().transform)
                        MergeController.Instance.Split(gridItemsToMerge.GetLastElement());
                    else
                        MergeController.Instance.Merge(objectHit.GetComponent<GridItem>());
                }
            }
        }
    }

    public void PressedDown()
    {
        isPressed = true;
    }

    public void PressedUp()
    {
        isPressed = false;
        OnRelease.Invoke();
    }
}
