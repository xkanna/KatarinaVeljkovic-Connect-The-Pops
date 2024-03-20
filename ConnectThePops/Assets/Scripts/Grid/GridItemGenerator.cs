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

    private void GenerateGridItem(SpawnPoint item)
    {
        var newGridItem = Instantiate(gridItem, item.transform.position, Quaternion.identity);
        scaler = StartCoroutine(SpawnScaleC(newGridItem, newGridItem.transform.localScale.x));
        newGridItem.transform.localScale = new Vector3(0, 0, 0);
        newGridItem.transform.parent = transform;
        newGridItem.InitGridItem(gridItemTypes.GetRandomItemType(), item.Ground);
        gridItemsOnScene.AddToList(newGridItem);

        StartCoroutine(MoveToBottomC(newGridItem));
    }

    private IEnumerator SpawnScaleC(GridItem item, float target)
    {
        var scale = 0f;
        var scaleTimer = 0f;
        var scaleTime = 0.5f;
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

    private IEnumerator MoveToBottomC(GridItem item)
    {
        var bottomPosition = SpawnPointController.Instance.GetBottomAvailableSpawnPoint(item.Ground);
        
        if (bottomPosition != null)
        {
            item.Ground = bottomPosition.Ground;
            Vector3 startPosition = item.transform.position;
            Vector3 targetPosition = bottomPosition.transform.position;
            float moveDuration = 0.3f; // Adjust as needed
        
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
}
