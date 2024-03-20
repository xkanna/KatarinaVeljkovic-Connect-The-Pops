using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnPointController : MonoBehaviour
{
    public static SpawnPointController Instance;
        
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    [SerializeField] private GridItemsOnScene gridItemsOnScene;

    public List<SpawnPoint> GetAllSpawnPoints()
    {
        List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>();

        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            allSpawnPoints.Add(item);
        }
        return allSpawnPoints;
    }
    
    public List<SpawnPoint> GetActiveSpawnPoints()
    {
        List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();

        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (item.IsActive)
                activeSpawnPoints.Add(item);
        }

        return activeSpawnPoints;
    }

    public List<SpawnPoint> GetNonActiveSpawnPoints()
    {
        List<SpawnPoint> activeSpawnPoints = new List<SpawnPoint>();

        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (item.IsActive == false)
                activeSpawnPoints.Add(item);
        }

        return activeSpawnPoints;
    }

    public List<SpawnPoint> GetTopAvailableSpawnPoints()
    {
        List<SpawnPoint> availableSpawnPoints = new List<SpawnPoint>();
        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (item.Ground.y == SpawnPointGenerator.Instance.MapSizeY - 1 && IsAvailable(item.Ground))
            {
                if (item.IsActive)
                {
                    availableSpawnPoints.Add(item);
                }
                else
                {
                    for (int i = 1; i < 4; i++)
                    {
                        var checkGround = new Vector3(item.Ground.x, item.ground.y - i, item.ground.z);
                        var checkItem = GetSpawnPointByGroundPosition(checkGround);
                        if (checkItem.IsActive && IsAvailable(checkItem.Ground))
                        {
                            availableSpawnPoints.Add(checkItem);
                            break;
                        }
                    }
                }
            }
        }
        
        
        return availableSpawnPoints;
    }

    public bool IsAvailable(Vector3 ground)
    {
        foreach (var item in gridItemsOnScene.GetAllElements())
        {
            if (item.Ground == ground) return false;
        }

        return true;
    }

    public SpawnPoint GetSpawnPointByGroundPosition(Vector3 ground)
    {
        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (item.Ground == ground) return item;
        }

        return null;
    }

    public bool IsSelectedBubbleNeighborWith(Vector3 selected, Vector3 withGround)
    {
        foreach (var item in GetSpawnPointByGroundPosition(selected).Neighbors)
        {
            if (item != null && item.Ground == withGround) return true;
        }

        return false;
    }

    public List<GridItem> GetGridItemNeighbors(SpawnPoint spawnPoint)
    {
        List<GridItem> neighbors = new List<GridItem>();

        foreach (var neighbor in spawnPoint.Neighbors)
        {
            foreach (var bubble in gridItemsOnScene.GetAllElements())
            {
                if (neighbor != null && neighbor.Ground == bubble.Ground)
                    neighbors.Add(bubble);
            }
        }

        return neighbors;
    }
    public List<SpawnPoint> GetActiveSpawnPointNeighbors(SpawnPoint spawnPoint)
    {
        List<SpawnPoint> neighbors = new List<SpawnPoint>();

        foreach (var neighbor in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (neighbor.IsActive)
            {
                foreach(var neighborCheck in neighbor.Neighbors)
                {
                    if (neighborCheck == spawnPoint)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }
    
    public SpawnPoint GetBottomAvailableSpawnPoint(Vector3 ground)
    {
        foreach (var spawnPoint in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (spawnPoint.Ground.x == ground.x && spawnPoint.Ground.y == ground.y - 1)
            {
                if (IsAvailable(spawnPoint.Ground))
                {
                    return spawnPoint;
                }
            }
        }
    
        return null;
    }
    
    public GridItem FindGridItemBySpawnPointGroundPosition(SpawnPoint spawnPoint)
    {
        foreach (var item in gridItemsOnScene.GetAllElements())
            if (item.Ground == spawnPoint.ground)
                return item;
        return null;
    }

    public List<GridItem> GetAllElements()
    {
        return gridItemsOnScene.GetAllElements();
    }

    public void ClearAll()
    {
        gridItemsOnScene.Clear();
    }
}
