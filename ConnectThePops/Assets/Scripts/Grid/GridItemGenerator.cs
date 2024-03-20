using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridItemGenerator : MonoBehaviour
{
    [SerializeField] private GridItemsOnScene gridItemsOnScene;
    [SerializeField] private GridItem gridItem;
    [SerializeField] private GridItemTypes gridItemTypes;
    private Coroutine spawnGridItemsCoroutine;
    private Coroutine scaler;
    private bool isFeverActive;
    private float spawnDelay = 0.05f;

    private void Start()
    {
        MergeController.Instance.OnMergeComplete.AddListener(SpawnNewGridItems);
        SpawnNewGridItems();
    }

    public void SpawnNewGridItems()
    {
        if (spawnGridItemsCoroutine == null)
            spawnGridItemsCoroutine = StartCoroutine(StartGeneratingC());
    }

    private IEnumerator StartGeneratingC()
    {
        yield return new WaitForSeconds(spawnDelay);
        do
        {
            yield return new WaitForSeconds(spawnDelay);
            Generate();
            foreach (var gridItem in gridItemsOnScene.GetAllElements())
            {
                CheckIfCanMoveBottom(gridItem);
            }
        } while (SpawnPointController.Instance.GetActiveSpawnPoints().Count != 0);

        yield return null;
        spawnGridItemsCoroutine = null;
    }

    private void Generate()
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
        scaler = StartCoroutine(SpawnScaleC(newGridItem, newGridItem.transform.localScale.x));
        newGridItem.transform.localScale = new Vector3(0, 0, 0);
        newGridItem.transform.parent = transform;
        newGridItem.InitGridItem(gridItemTypes.GetRandomItemType(), spawnPoint.Ground);
        gridItemsOnScene.AddToList(newGridItem);

        //CheckIfCanMoveBottom(newGridItem);
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
        var scaleTime = 1f;
        while (scaleTimer < scaleTime)
        {
            scale = EasingFunction.EaseOutElastic(0, target, scaleTimer / scaleTime);
            scaleTimer += Time.deltaTime;
            item.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        item.transform.localScale = new Vector3(target, target, target);
        yield return null;
    }

    private IEnumerator MoveToBottomC(GridItem item, SpawnPoint bottomPosition)
    {
            item.Ground = bottomPosition.Ground;
            Vector3 startPosition = item.transform.position;
            Vector3 targetPosition = bottomPosition.transform.position;
            float moveDuration = 0.1f; // Adjust as needed
        
            float elapsedTime = 0f;
            while (elapsedTime < moveDuration)
            {
                item.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            item.transform.position = targetPosition;
    }
}
