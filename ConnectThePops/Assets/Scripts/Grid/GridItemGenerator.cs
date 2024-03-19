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
    private Coroutine _coroutine;
    private Coroutine _scaler;
    private bool isFeverActive;
    private float spawnDelay = 0.01f;

    private void Start()
    { 
        StartGenerate();
    }

    public void StartGenerate()
    {
        if (_coroutine == null)
            _coroutine = StartCoroutine(KeepGenerating());
    }

    private IEnumerator KeepGenerating()
    {
        yield return new WaitForSeconds(spawnDelay);
        do
        {
            yield return new WaitForSeconds(spawnDelay);
            Generate();
        } while (SpawnPointController.Instance.GetTopAvailableSpawnPoints().Count != 0);
        
        yield return null;
        _coroutine = null;
        spawnDelay = 0.07f;
    }

    public void Generate()
    {
        var availableTopSpawnPoints = SpawnPointController.Instance.GetTopAvailableSpawnPoints();
        Debug.LogError(availableTopSpawnPoints.Count);
        foreach (var item in availableTopSpawnPoints)
        {
            if (item.IsActive == false) continue;
            GenerateBubble(item);
            break;
        }
    }

    public void GenerateBubble(SpawnPoint item)
    {
        Debug.LogError("gen bubble");
        var create = Instantiate(gridItem, item.transform.position, Quaternion.identity);
        _scaler = StartCoroutine(SpawnScaler(create, create.transform.localScale.x));
        create.transform.localScale = new Vector3(0, 0, 0);
        create.transform.parent = transform;
        create.InitGridItem(gridItemTypes.GetRandomItemType(), item.Ground);
        gridItemsOnScene.AddToList(create);
    }

    private IEnumerator SpawnScaler(GridItem item, float target)
    {
        var scale = 0f;
        var scaleTimer = 0f;
        var scaleTime = 0.5f;
        while (scaleTimer < scaleTime)
        {
            scale = EasingFunction.EaseOutElastic(0, target, scaleTimer/scaleTime);
            scaleTimer += Time.deltaTime;
            item.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        item.transform.localScale = new Vector3(target, target, target);
        yield return null;
    }
}
