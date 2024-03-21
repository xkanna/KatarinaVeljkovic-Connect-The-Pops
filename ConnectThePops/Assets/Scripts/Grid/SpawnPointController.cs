using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointController : MonoBehaviour
{
    [SerializeField] private GridItemsOnScene gridItemsOnScene;
    
    public static SpawnPointController Instance;
        
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    public List<SpawnPoint> GetActiveSpawnPoints()
    {
        var activeSpawnPoints = new List<SpawnPoint>();

        foreach (var item in SpawnPointGenerator.Instance.SpawnPoints)
        {
            if (item.IsActive)
                activeSpawnPoints.Add(item);
        }

        return activeSpawnPoints;
    }

    public List<SpawnPoint> GetTopAvailableSpawnPoints()
    {
        var availableSpawnPoints = new List<SpawnPoint>();
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
                    for (int i = 1; i < SpawnPointGenerator.Instance.MapSizeX - 1; i++)
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

    private bool IsAvailable(Vector3 ground)
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
}
