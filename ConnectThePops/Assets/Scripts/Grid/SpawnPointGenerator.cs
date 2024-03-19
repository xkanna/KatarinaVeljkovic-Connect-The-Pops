using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointGenerator : MonoBehaviour
{
    public static SpawnPointGenerator Instance;
        
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    
    [SerializeField] private int mapSizeX = 5;
    [SerializeField] private int mapSizeY = 5;
    [SerializeField] private SpawnPoint spawnPointPrefab;
    private SpawnPoint[,] spawnPoints;
    private int[,] mapFilled;
    public SpawnPoint[,] SpawnPoints { get => spawnPoints; }
    public int MapSizeX { get => mapSizeX; }
    public int MapSizeY { get => mapSizeY; }

    void Start()
    {
        Generate();
    }

    private void Generate()
    {
        mapFilled = new int[MapSizeX, MapSizeY];
        spawnPoints = new SpawnPoint[MapSizeX, MapSizeY];
        GenerateStartingGrid();

        foreach (var item in SpawnPoints)
        {
            AssignTileNeighbors(item);
        }
        
    }

    void GenerateStartingGrid()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                if (mapFilled[x, y] == 0)
                {
                    Vector3 tilePosition = new Vector3(x, y, 0);
                    
                    SpawnPoint newTile;

                    newTile = Instantiate(spawnPointPrefab, tilePosition, Quaternion.identity);
                    
                    newTile.transform.SetParent(transform, false);
                    newTile.Ground = tilePosition;

                    SpawnPoints[(int)tilePosition.x, (int)tilePosition.y] = newTile;
                }
                else { continue; }
            }
        }
    }

    void AssignTileNeighbors(SpawnPoint SP)
    {
        if (SP.Ground.y + 1 < MapSizeY)
            SP.Neighbor_UP = SpawnPoints[(int)SP.Ground.x, (int)SP.Ground.y + 1];

        if (SP.Ground.y - 1 > -1)
            SP.Neighbor_DOWN = SpawnPoints[(int)SP.Ground.x, (int)SP.Ground.y - 1];

        if (SP.Ground.x + 1 < MapSizeX)
            SP.Neighbor_RIGHT = SpawnPoints[(int)SP.Ground.x + 1, (int)SP.Ground.y];

        if (SP.Ground.x - 1 > -1)
            SP.Neighbor_LEFT = SpawnPoints[(int)SP.Ground.x - 1, (int)SP.Ground.y];


        if (SP.Ground.x - 1 > -1 && SP.Ground.y + 1 < MapSizeY)
            SP.Neighbor_UP_LEFT = SpawnPoints[(int)SP.Ground.x - 1, (int)SP.Ground.y + 1];

        if (SP.Ground.x + 1 < MapSizeX && SP.Ground.y + 1 < MapSizeY)
            SP.Neighbor_UP_RIGHT = SpawnPoints[(int)SP.Ground.x + 1, (int)SP.Ground.y + 1];

        if (SP.Ground.x - 1 > -1 && SP.Ground.y - 1 > -1)
            SP.Neighbor_DOWN_LEFT = SpawnPoints[(int)SP.Ground.x - 1, (int)SP.Ground.y - 1];

        if (SP.Ground.x + 1 < MapSizeX && SP.Ground.y - 1 > -1)
            SP.Neighbor_DOWN_RIGHT = SpawnPoints[(int)SP.Ground.x + 1, (int)SP.Ground.y - 1];
    }
}
