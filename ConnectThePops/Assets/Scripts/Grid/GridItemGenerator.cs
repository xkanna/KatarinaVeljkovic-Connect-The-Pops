using System;
using System.Collections;
using UnityEngine;

public class GridItemGenerator : MonoBehaviour
{
    [SerializeField] private GridItemsOnScene gridItemsOnScene;
    [SerializeField] private GridItem gridItem;
    [SerializeField] private GridItemTypes gridItemTypes;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float spawnDelay = 0.05f;
    private Coroutine spawnGridItemsCoroutine;

    private void Start()
    {
        MergeController.Instance.OnMergeComplete.AddListener(SpawnNewGridItems);
        SpawnNewGridItems();
    }

    private void SpawnNewGridItems()
    {
        if (spawnGridItemsCoroutine == null)
            spawnGridItemsCoroutine = StartCoroutine(StartGeneratingGridItemsC());
    }

    private IEnumerator StartGeneratingGridItemsC()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        do
        {
            yield return new WaitForSeconds(spawnDelay);
            GenerateGridItems();
            foreach (var gridItemOnScene in gridItemsOnScene.GetAllElements())
            {
                CheckIfCanMoveBottom(gridItemOnScene);
            }
        } 
        while (SpawnPointController.Instance.GetActiveSpawnPoints().Count != 0);

        yield return null;
        spawnGridItemsCoroutine = null;
    }

    private void GenerateGridItems()
    {
        var availableTopSpawnPoints = SpawnPointController.Instance.GetTopAvailableSpawnPoints();
        
        foreach (var item in availableTopSpawnPoints)
        {
            if (item.IsActive == false) continue;
            GenerateGridItem(item);
            break;
        }
    }

    private void GenerateGridItem(SpawnPoint spawnPoint)
    {
        var newGridItem = Instantiate(gridItem, spawnPoint.transform.position, Quaternion.identity);
        StartCoroutine(SpawnScaleC(newGridItem, newGridItem.transform.localScale.x));
        newGridItem.transform.localScale = new Vector3(0, 0, 0);
        newGridItem.transform.parent = transform;
        newGridItem.InitGridItem(gridItemTypes.GetRandomItemType(), spawnPoint.Ground);
        gridItemsOnScene.AddToList(newGridItem);
    }

    private void CheckIfCanMoveBottom(GridItem gridItem)
    {
        var bottomPosition = SpawnPointController.Instance.GetBottomAvailableSpawnPoint(gridItem.Ground);
        if (bottomPosition != null)
        {
            StartCoroutine(MoveToBottomC(gridItem, bottomPosition));
        }
    }

    private IEnumerator SpawnScaleC(GridItem item, float target)
    {
        var scale = 0f;
        var scaleTimer = 0f;
        var scaleTime = 0.3f;
        while (scaleTimer < scaleTime)
        {
            //scale = EasingFunction.EaseOutElastic(0, target, scaleTimer / scaleTime);
            scaleTimer += Time.deltaTime;
            item.transform.localScale = Vector3.Lerp(gridItem.transform.position, Vector3.one, scaleTimer / scaleTime);
            yield return null;
        }
        item.transform.localScale = new Vector3(target, target, target);
        yield return null;
    }

    private IEnumerator MoveToBottomC(GridItem item, SpawnPoint bottomPosition)
    {
        //Moving to desired position
        item.Ground = bottomPosition.Ground;
        var startPosition = item.transform.position;
        var targetPosition = bottomPosition.transform.position;
        var moveDuration = 0.15f; 
    
        var elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            item.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        item.transform.position = targetPosition;
        
        //Small bounce on the end of moving
        var scaleTime = 0.4f;
        var elapsedScaleTime = 0f;
        while (elapsedScaleTime < scaleTime)
        {
            item.transform.localScale = new Vector3(1,animationCurve.Evaluate(elapsedScaleTime / scaleTime), 1);
            elapsedScaleTime += Time.deltaTime;
            yield return null;
        }
        item.transform.localScale = Vector3.one;
    }
}
